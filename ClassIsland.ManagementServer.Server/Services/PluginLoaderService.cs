using System.Reflection;
using System.Runtime.Loader;
using ClassIsland.ManagementServer.Server.Abstractions.Plugin;

namespace ClassIsland.ManagementServer.Server.Services;

public class PluginLoaderService
{
    private readonly ILogger<PluginLoaderService> _logger;
    private readonly PluginManagerService _pluginManager;
    private readonly IServiceProvider _serviceProvider;

    private readonly Dictionary<string, PluginLoadContext> _loadedContexts = new();

    public PluginLoaderService(
        ILogger<PluginLoaderService> logger,
        PluginManagerService pluginManager,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _pluginManager = pluginManager;
        _serviceProvider = serviceProvider;
    }

    public async Task LoadPluginsFromDirectoryAsync(string directory)
    {
        if (!Directory.Exists(directory))
        {
            _logger.LogWarning("Plugin directory does not exist: {}", directory);
            return;
        }

        var dllFiles = Directory.GetFiles(directory, "*.dll", SearchOption.AllDirectories);
        foreach (var dllPath in dllFiles)
        {
            await LoadPluginFromAssemblyAsync(dllPath);
        }
    }

    public async Task<List<IServerPlugin>> LoadPluginFromAssemblyAsync(string assemblyPath)
    {
        var loadedPlugins = new List<IServerPlugin>();
        
        try
        {
            var absolutePath = Path.GetFullPath(assemblyPath);
            if (!File.Exists(absolutePath))
            {
                _logger.LogWarning("Assembly file not found: {}", absolutePath);
                return loadedPlugins;
            }

            var loadContext = new PluginLoadContext(absolutePath);
            var assembly = loadContext.LoadFromAssemblyPath(absolutePath);

            var pluginTypes = assembly.GetTypes()
                .Where(t => typeof(IServerPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .ToList();

            if (pluginTypes.Count == 0)
            {
                _logger.LogInformation("No plugins found in assembly: {}", absolutePath);
                loadContext.Unload();
                return loadedPlugins;
            }

            foreach (var pluginType in pluginTypes)
            {
                try
                {
                    var plugin = CreatePluginInstance(pluginType);
                    if (plugin != null)
                    {
                        if (await _pluginManager.LoadPluginAsync(plugin))
                        {
                            _loadedContexts[plugin.Identifier] = loadContext;
                            loadedPlugins.Add(plugin);
                            _logger.LogInformation("Loaded plugin {} from {}", plugin.Identifier, absolutePath);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to create plugin instance from type {}", pluginType.FullName);
                }
            }

            if (loadedPlugins.Count == 0)
            {
                loadContext.Unload();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load plugins from assembly: {}", assemblyPath);
        }

        return loadedPlugins;
    }

    public async Task<bool> UnloadPluginAsync(string identifier, TimeSpan timeout)
    {
        var result = await _pluginManager.UnloadPluginAsync(identifier, timeout);
        
        if (result && _loadedContexts.TryGetValue(identifier, out var context))
        {
            context.Unload();
            _loadedContexts.Remove(identifier);
            _logger.LogInformation("Unloaded assembly for plugin {}", identifier);
        }

        return result;
    }

    public async Task<bool> HotReloadPluginAsync(string identifier, string newAssemblyPath, TimeSpan timeout)
    {
        _logger.LogInformation("Hot reloading plugin {}", identifier);
        
        var unloadResult = await UnloadPluginAsync(identifier, timeout);
        if (!unloadResult)
        {
            _logger.LogWarning("Failed to unload plugin {} for hot reload", identifier);
            return false;
        }

        var loadedPlugins = await LoadPluginFromAssemblyAsync(newAssemblyPath);
        var reloadedPlugin = loadedPlugins.FirstOrDefault(p => p.Identifier == identifier);
        
        if (reloadedPlugin != null)
        {
            _logger.LogInformation("Successfully hot reloaded plugin {}", identifier);
            return true;
        }

        _logger.LogWarning("Hot reload failed - new assembly does not contain plugin {}", identifier);
        return false;
    }

    private IServerPlugin? CreatePluginInstance(Type pluginType)
    {
        var constructors = pluginType.GetConstructors();
        
        foreach (var constructor in constructors.OrderByDescending(c => c.GetParameters().Length))
        {
            var parameters = constructor.GetParameters();
            var args = new object?[parameters.Length];
            var canCreate = true;

            for (int i = 0; i < parameters.Length; i++)
            {
                var paramType = parameters[i].ParameterType;
                var service = _serviceProvider.GetService(paramType);
                
                if (service != null)
                {
                    args[i] = service;
                }
                else if (parameters[i].HasDefaultValue)
                {
                    args[i] = parameters[i].DefaultValue;
                }
                else
                {
                    canCreate = false;
                    break;
                }
            }

            if (canCreate)
            {
                return (IServerPlugin)constructor.Invoke(args);
            }
        }

        if (pluginType.GetConstructor(Type.EmptyTypes) != null)
        {
            return (IServerPlugin)Activator.CreateInstance(pluginType)!;
        }

        _logger.LogWarning("Cannot create instance of plugin type {} - no suitable constructor", pluginType.FullName);
        return null;
    }
}

internal class PluginLoadContext : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver _resolver;

    public PluginLoadContext(string pluginPath) : base(isCollectible: true)
    {
        _resolver = new AssemblyDependencyResolver(pluginPath);
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        if (assemblyPath != null)
        {
            return LoadFromAssemblyPath(assemblyPath);
        }

        return null;
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        if (libraryPath != null)
        {
            return LoadUnmanagedDllFromPath(libraryPath);
        }

        return IntPtr.Zero;
    }
}
