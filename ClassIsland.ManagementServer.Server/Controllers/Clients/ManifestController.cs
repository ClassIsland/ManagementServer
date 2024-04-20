using ClassIsland.Core.Models.Management;
using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClassIsland.ManagementServer.Server.Controllers.Clients;

[ApiController]
[Route("api/v1/client/{cuid}/manifest")]
public class ManifestController(ManagementServerContext dataContext, ObjectsUpdateNotifyService objectsUpdateNotifyService) : ControllerBase
{
    private ManagementServerContext DataContext { get; } = dataContext;

    private ObjectsUpdateNotifyService ObjectsUpdateNotifyService { get; } = objectsUpdateNotifyService;

    [HttpGet]
    public IActionResult GetManifest([FromRoute] string cuid)
    {
        if (!DataContext.Clients.Any(i => i.Cuid == cuid))
        {
            return NotFound();
        }

        return Ok(new ManagementManifest());
    }
    
}