using System.Collections.Concurrent;
using ClassIsland.ManagementServer.Server.Abstractions.Plugin;
using ClassIsland.ManagementServer.Server.Models.CyreneMsp;
using ClassIsland.Shared.Protobuf.Enum;
using ClassIsland.Shared.Protobuf.Server;
using Google.Protobuf;

namespace ClassIsland.ManagementServer.Server.Services;

public class PluginManagerService
{
    private readonly ILogger<PluginManagerService> _logger;
    private readonly CyreneMspConnectionService _connectionService;
    private readonly IServiceProvider _serviceProvider;
    
    public ConcurrentDictionary<string, IServerPlugin> Plugins { get; } = new();

    public ConcurrentDictionary<string, ConcurrentDictionary<Guid, bool>> PendingUnloadConfirmations { get; } = new();

    public ConcurrentDictionary<string, TaskCompletionSource<bool>> PendingUnloads { get; } = new();

    public PluginManagerService(
        ILogger<PluginManagerService> logger,
        CyreneMspConnectionService connectionService,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _connectionService = connectionService;
        _serviceProvider = serviceProvider;
    }

    public async Task<bool> LoadPluginAsync(IServerPlugin plugin)
    {
        if (Plugins.ContainsKey(plugin.Identifier))
        {
            _logger.LogWarning("Plugin {} is already loaded", plugin.Identifier);
            return false;
        }

        try
        {
            await plugin.OnLoadAsync();
            Plugins[plugin.Identifier] = plugin;
            _logger.LogInformation("Plugin {} v{} loaded successfully", plugin.Identifier, plugin.Version);

            await NotifyClientsPluginStateChangeAsync(plugin.Identifier, plugin.Version, isLoading: true);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load plugin {}", plugin.Identifier);
            return false;
        }
    }

    public async Task<bool> UnloadPluginAsync(string identifier, TimeSpan timeout)
    {
        if (!Plugins.TryGetValue(identifier, out var plugin))
        {
            _logger.LogWarning("Plugin {} is not loaded", identifier);
            return false;
        }

        var activeSessions = _connectionService.Sessions
            .Where(s => s.Value.IsActivated)
            .Select(s => s.Key)
            .ToList();

        if (activeSessions.Count == 0) // 无活跃客户端，直接卸载插件
        {
            return await CompleteUnloadAsync(identifier, plugin);
        }

        var pendingConfirmations = new ConcurrentDictionary<Guid, bool>();
        foreach (var clientUid in activeSessions)
        {
            pendingConfirmations[clientUid] = false;
        }
        PendingUnloadConfirmations[identifier] = pendingConfirmations;

        var tcs = new TaskCompletionSource<bool>();
        PendingUnloads[identifier] = tcs;

        await NotifyClientsPluginDisableAsync(identifier, "Plugin is being unloaded");

        using var cts = new CancellationTokenSource(timeout);
        try
        {
            var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(Timeout.Infinite, cts.Token));
            if (completedTask == tcs.Task && await tcs.Task)
            {
                return await CompleteUnloadAsync(identifier, plugin);
            }
            else
            {
                _logger.LogWarning("Timeout waiting for clients to confirm plugin {} disable", identifier);
                // 超时直接强制卸载
                return await CompleteUnloadAsync(identifier, plugin);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Plugin {} unload timed out after {} seconds, forcing unload", identifier, timeout.TotalSeconds);
            return await CompleteUnloadAsync(identifier, plugin);
        }
        finally
        {
            PendingUnloadConfirmations.TryRemove(identifier, out _);
            PendingUnloads.TryRemove(identifier, out _);
        }
    }

    public void HandlePluginDisableAck(Guid clientUid, string pluginIdentifier, bool success)
    {
        if (!PendingUnloadConfirmations.TryGetValue(pluginIdentifier, out var confirmations))
        {
            return;
        }

        if (confirmations.TryGetValue(clientUid, out _))
        {
            confirmations[clientUid] = success;
            _logger.LogInformation("Client {} confirmed disable of plugin {}: {}", clientUid, pluginIdentifier, success);

            if (confirmations.Values.All(v => v))
            {
                if (PendingUnloads.TryGetValue(pluginIdentifier, out var tcs))
                {
                    tcs.TrySetResult(true);
                }
            }
        }
    }
    
    public void HandlePluginEnableAck(Guid clientUid, string pluginIdentifier, bool success)
    {
        _logger.LogInformation("Client {} confirmed enable of plugin {}: {}", clientUid, pluginIdentifier, success);
    }

    public (List<string> compatible, List<string> incompatible) CheckPluginCompatibility(
        IEnumerable<(string identifier, string version, bool isPureLocal)> clientPlugins)
    {
        var compatible = new List<string>();
        var incompatible = new List<string>();

        foreach (var (identifier, version, isPureLocal) in clientPlugins)
        {
            if (isPureLocal)
            {
                compatible.Add(identifier);
            }
            else if (Plugins.TryGetValue(identifier, out var serverPlugin))
            {
                if (serverPlugin.IsCompatible(version))
                {
                    compatible.Add(identifier);
                }
                else
                {
                    incompatible.Add(identifier);
                }
            }
            else
            {
                incompatible.Add(identifier);
            }
        }

        return (compatible, incompatible);
    }

    public IEnumerable<IServerPlugin> GetAllPlugins() => Plugins.Values;

    public async Task<PluginMessageResponse> HandlePluginMessageAsync(
        Guid clientId, string pluginIdentifier, string messageType, byte[] payload)
    {
        if (!Plugins.TryGetValue(pluginIdentifier, out var plugin))
        {
            _logger.LogWarning("Plugin {} not found", pluginIdentifier);
            return new PluginMessageResponse 
            { 
                RetCode = (int)Retcode.PluginNotFound,
                MessageType = "error",
                Payload = Array.Empty<byte>()
            };
        }

        try
        {
            return await plugin.HandleMessageAsync(clientId, messageType, payload);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling message for plugin {}", pluginIdentifier);
            return new PluginMessageResponse
            {
                RetCode = (int)Retcode.ServerInternalError,
                MessageType = "error",
                Payload = Array.Empty<byte>()
            };
        }
    }

    public async Task SendPluginMessageToClientAsync(Guid clientUid, string pluginIdentifier, string messageType, byte[] payload)
    {
        if (!_connectionService.Sessions.TryGetValue(clientUid, out var session) || 
            !session.IsActivated || 
            session.CommandFlowWriter == null)
        {
            _logger.LogWarning("Client {} is not connected or session not active", clientUid);
            return;
        }

        var response = new PluginServerToClientRsp
        {
            PluginIdentifier = pluginIdentifier,
            MessageType = messageType,
            Payload = ByteString.CopyFrom(payload)
        };

        await session.CommandFlowWriter.WriteAsync(new ClientCommandDeliverScRsp
        {
            RetCode = Retcode.Success,
            Type = CommandTypes.PluginMessage,
            Payload = response.ToByteString()
        });
    }

    public async Task BroadcastPluginMessageAsync(string pluginIdentifier, string messageType, byte[] payload)
    {
        var activeSessions = _connectionService.Sessions
            .Where(s => s.Value.IsActivated && s.Value.CommandFlowWriter != null)
            .ToList();

        var response = new PluginServerToClientRsp
        {
            PluginIdentifier = pluginIdentifier,
            MessageType = messageType,
            Payload = ByteString.CopyFrom(payload)
        };

        var rsp = new ClientCommandDeliverScRsp
        {
            RetCode = Retcode.Success,
            Type = CommandTypes.PluginMessage,
            Payload = response.ToByteString()
        };

        foreach (var (clientUid, session) in activeSessions)
        {
            try
            {
                await session.CommandFlowWriter!.WriteAsync(rsp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send plugin message to client {}", clientUid);
            }
        }
    }

    private async Task<bool> CompleteUnloadAsync(string identifier, IServerPlugin plugin)
    {
        try
        {
            await plugin.OnUnloadAsync();
            Plugins.TryRemove(identifier, out _);
            _logger.LogInformation("Plugin {} unloaded successfully", identifier);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during plugin {} unload", identifier);
            Plugins.TryRemove(identifier, out _);
            return false;
        }
    }

    private async Task NotifyClientsPluginStateChangeAsync(string pluginIdentifier, string version, bool isLoading)
    {
        var notification = new PluginStateChangeNotification
        {
            PluginIdentifier = pluginIdentifier,
            Version = version,
            IsLoading = isLoading
        };

        var rsp = new ClientCommandDeliverScRsp
        {
            RetCode = Retcode.Success,
            Type = CommandTypes.PluginStateChange,
            Payload = notification.ToByteString()
        };

        var activeSessions = _connectionService.Sessions
            .Where(s => s.Value.IsActivated && s.Value.CommandFlowWriter != null)
            .ToList();

        foreach (var (clientUid, session) in activeSessions)
        {
            try
            {
                await session.CommandFlowWriter!.WriteAsync(rsp);
                _logger.LogInformation("Notified client {} about plugin {} state change", clientUid, pluginIdentifier);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to notify client {} about plugin state change", clientUid);
            }
        }
    }

    private async Task NotifyClientsPluginDisableAsync(string pluginIdentifier, string reason)
    {
        var request = new PluginDisableRequest
        {
            PluginIdentifier = pluginIdentifier,
            Reason = reason
        };

        var rsp = new ClientCommandDeliverScRsp
        {
            RetCode = Retcode.Success,
            Type = CommandTypes.PluginDisableRequest,
            Payload = request.ToByteString()
        };

        var activeSessions = _connectionService.Sessions
            .Where(s => s.Value.IsActivated && s.Value.CommandFlowWriter != null)
            .ToList();

        foreach (var (clientUid, session) in activeSessions)
        {
            try
            {
                await session.CommandFlowWriter!.WriteAsync(rsp);
                _logger.LogInformation("Requested client {} to disable plugin {}", clientUid, pluginIdentifier);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to request client {} to disable plugin", clientUid);
            }
        }
    }

    public async Task NotifyClientsPluginEnableAsync(string pluginIdentifier)
    {
        var request = new PluginEnableRequest
        {
            PluginIdentifier = pluginIdentifier
        };

        var rsp = new ClientCommandDeliverScRsp
        {
            RetCode = Retcode.Success,
            Type = CommandTypes.PluginEnableRequest,
            Payload = request.ToByteString()
        };

        var activeSessions = _connectionService.Sessions
            .Where(s => s.Value.IsActivated && s.Value.CommandFlowWriter != null)
            .ToList();

        foreach (var (clientUid, session) in activeSessions)
        {
            try
            {
                await session.CommandFlowWriter!.WriteAsync(rsp);
                _logger.LogInformation("Requested client {} to enable plugin {}", clientUid, pluginIdentifier);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to request client {} to enable plugin", clientUid);
            }
        }
    }
}
