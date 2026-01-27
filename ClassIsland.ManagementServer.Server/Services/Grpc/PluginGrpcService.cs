using ClassIsland.ManagementServer.Server.Abstractions.Plugin;
using ClassIsland.Shared.Protobuf.Client;
using ClassIsland.Shared.Protobuf.Enum;
using ClassIsland.Shared.Protobuf.Server;
using ClassIsland.Shared.Protobuf.Service;
using Google.Protobuf;
using Grpc.Core;

namespace ClassIsland.ManagementServer.Server.Services.Grpc;

public class PluginGrpcService(
    ILogger<PluginGrpcService> logger,
    PluginManagerService pluginManagerService,
    CyreneMspConnectionService connectionService) : PluginService.PluginServiceBase
{
    private ILogger<PluginGrpcService> Logger { get; } = logger;
    private PluginManagerService PluginManagerService { get; } = pluginManagerService;
    private CyreneMspConnectionService ConnectionService { get; } = connectionService;

    public override Task<PluginRegisterRsp> RegisterPlugins(PluginRegisterReq request, ServerCallContext context)
    {
        var clientPlugins = request.Plugins.Select(p => 
            (p.Identifier, p.Version, p.IsPureLocal)).ToList();
        
        var (compatible, incompatible) = PluginManagerService.CheckPluginCompatibility(clientPlugins);
        
        var response = new PluginRegisterRsp();
        response.CompatiblePlugins.AddRange(compatible);
        response.IncompatiblePlugins.AddRange(incompatible);
        
        Logger.LogInformation("插件兼容性检查: {} 兼容, {} 不兼容", 
            compatible.Count, incompatible.Count);
        
        return Task.FromResult(response);
    }

    public override Task<PluginListRsp> GetServerPlugins(GetServerPluginsReq request, ServerCallContext context)
    {
        var response = new PluginListRsp();
        
        foreach (var plugin in PluginManagerService.GetAllPlugins())
        {
            response.Plugins.Add(new ServerPluginInfo
            {
                Identifier = plugin.Identifier,
                Version = plugin.Version,
                Name = plugin.Name,
                Description = plugin.Description
            });
        }
        
        Logger.LogInformation("获取到 {} 个服务端插件", response.Plugins.Count);
        
        return Task.FromResult(response);
    }

    public override async Task<PluginClientToServerRsp> SendPluginMessage(PluginClientToServerReq request, ServerCallContext context)
    {
        if (!TryGetClientUid(context, out var clientUid))
        {
            return CreateErrorResponse(request.PluginIdentifier, (int)Retcode.InvalidRequest);
        }

        var result = await PluginManagerService.HandlePluginMessageAsync(
            clientUid,
            request.PluginIdentifier,
            request.MessageType,
            request.Payload.ToByteArray());

        return new PluginClientToServerRsp
        {
            RetCode = result.RetCode,
            PluginIdentifier = request.PluginIdentifier,
            MessageType = result.MessageType,
            Payload = ByteString.CopyFrom(result.Payload)
        };
    }

    public override Task<PluginClientToServerRsp> AcknowledgePluginDisable(PluginDisableAck request, ServerCallContext context)
    {
        if (!TryGetClientUid(context, out var clientUid))
        {
            return Task.FromResult(CreateErrorResponse(request.PluginIdentifier, (int)Retcode.InvalidRequest));
        }

        PluginManagerService.HandlePluginDisableAck(clientUid, request.PluginIdentifier, request.Success);

        return Task.FromResult(new PluginClientToServerRsp
        {
            RetCode = (int)Retcode.Success,
            PluginIdentifier = request.PluginIdentifier,
            MessageType = "ack"
        });
    }

    /// <summary>
    /// Handle client acknowledgement of plugin enable
    /// </summary>
    public override Task<PluginClientToServerRsp> AcknowledgePluginEnable(PluginEnableAck request, ServerCallContext context)
    {
        if (!TryGetClientUid(context, out var clientUid))
        {
            return Task.FromResult(CreateErrorResponse(request.PluginIdentifier, (int)Retcode.InvalidRequest));
        }

        PluginManagerService.HandlePluginEnableAck(clientUid, request.PluginIdentifier, request.Success);

        return Task.FromResult(new PluginClientToServerRsp
        {
            RetCode = (int)Retcode.Success,
            PluginIdentifier = request.PluginIdentifier,
            MessageType = "ack"
        });
    }

    /// <summary>
    /// Try to extract client UID from request headers
    /// </summary>
    private static bool TryGetClientUid(ServerCallContext context, out Guid clientUid)
    {
        return Guid.TryParse(context.RequestHeaders.GetValue("cuid"), out clientUid);
    }

    /// <summary>
    /// Create a standard error response
    /// </summary>
    private static PluginClientToServerRsp CreateErrorResponse(string pluginIdentifier, int retCode)
    {
        return new PluginClientToServerRsp
        {
            RetCode = retCode,
            PluginIdentifier = pluginIdentifier,
            MessageType = "error"
        };
    }
}
