using ClassIsland.ManagementServer.Server.Authorization;
using ClassIsland.ManagementServer.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClassIsland.ManagementServer.Server.Controllers;

[ApiController]
[Authorize(Roles = Roles.Admin)]
[Route("api/v1/plugins/")]
public class PluginController(PluginManagerService pluginManagerService, PluginLoaderService pluginLoaderService) : ControllerBase
{
    private PluginManagerService PluginManagerService { get; } = pluginManagerService;
    private PluginLoaderService PluginLoaderService { get; } = pluginLoaderService;

    [HttpGet]
    public IActionResult GetPlugins()
    {
        var plugins = PluginManagerService.GetAllPlugins().Select(p => new
        {
            p.Identifier,
            p.Name,
            p.Description,
            p.Version,
            p.MinClientVersion,
            p.MaxClientVersion
        });
        return Ok(plugins);
    }

    [HttpGet("{identifier}")]
    public IActionResult GetPlugin(string identifier)
    {
        var plugin = PluginManagerService.Plugins.GetValueOrDefault(identifier);
        if (plugin == null)
        {
            return NotFound(new { message = $"Plugin '{identifier}' not found" });
        }

        return Ok(new
        {
            plugin.Identifier,
            plugin.Name,
            plugin.Description,
            plugin.Version,
            plugin.MinClientVersion,
            plugin.MaxClientVersion
        });
    }

    [HttpPost("load")]
    public async Task<IActionResult> LoadPlugin([FromBody] LoadPluginRequest request)
    {
        var plugins = await PluginLoaderService.LoadPluginFromAssemblyAsync(request.AssemblyPath);
        if (plugins.Count > 0)
        {
            return Ok(new 
            { 
                message = $"Loaded {plugins.Count} plugin(s)",
                plugins = plugins.Select(p => new { p.Identifier, p.Name, p.Version })
            });
        }
        return BadRequest(new { message = "No plugins found in the specified assembly" });
    }

    [HttpPost("load-directory")]
    public async Task<IActionResult> LoadPluginsFromDirectory([FromBody] LoadPluginDirectoryRequest request)
    {
        await PluginLoaderService.LoadPluginsFromDirectoryAsync(request.Directory);
        return Ok(new { message = $"Loaded plugins from directory: {request.Directory}" });
    }

    [HttpDelete("{identifier}")]
    public async Task<IActionResult> UnloadPlugin(string identifier, [FromQuery] int timeoutSeconds = 30)
    {
        var result = await PluginLoaderService.UnloadPluginAsync(identifier, TimeSpan.FromSeconds(timeoutSeconds));
        if (result)
        {
            return Ok(new { message = $"Plugin '{identifier}' unloaded successfully" });
        }
        return BadRequest(new { message = $"Failed to unload plugin '{identifier}'" });
    }

    [HttpPost("{identifier}/hot-reload")]
    public async Task<IActionResult> HotReloadPlugin(string identifier, [FromBody] HotReloadRequest request, [FromQuery] int timeoutSeconds = 30)
    {
        var result = await PluginLoaderService.HotReloadPluginAsync(identifier, request.NewAssemblyPath, TimeSpan.FromSeconds(timeoutSeconds));
        if (result)
        {
            return Ok(new { message = $"Plugin '{identifier}' hot reloaded successfully" });
        }
        return BadRequest(new { message = $"Failed to hot reload plugin '{identifier}'" });
    }

    [HttpPost("{identifier}/notify-enable")]
    public async Task<IActionResult> NotifyPluginEnable(string identifier)
    {
        if (!PluginManagerService.Plugins.ContainsKey(identifier))
        {
            return NotFound(new { message = $"Plugin '{identifier}' not found" });
        }

        await PluginManagerService.NotifyClientsPluginEnableAsync(identifier);
        return Ok(new { message = $"Clients notified to enable plugin '{identifier}'" });
    }

    [HttpGet("{identifier}/pending-unload")]
    public IActionResult GetPendingUnload(string identifier)
    {
        if (!PluginManagerService.PendingUnloadConfirmations.TryGetValue(identifier, out var confirmations))
        {
            return NotFound(new { message = $"No pending unload for plugin '{identifier}'" });
        }

        return Ok(new
        {
            PluginIdentifier = identifier,
            TotalClients = confirmations.Count,
            ConfirmedClients = confirmations.Count(c => c.Value),
            PendingClients = confirmations.Where(c => !c.Value).Select(c => c.Key.ToString())
        });
    }
}

public class LoadPluginRequest
{
    public string AssemblyPath { get; set; } = string.Empty;
}

public class LoadPluginDirectoryRequest
{
    public string Directory { get; set; } = string.Empty;
}

public class HotReloadRequest
{
    public string NewAssemblyPath { get; set; } = string.Empty;
}
