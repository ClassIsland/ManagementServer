using ClassIsland.Core.Models.Management;
using ClassIsland.ManagementServer.Server.Context;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ClassIsland.ManagementServer.Server.Controllers;

[ApiController]
[Route("api/v1/client/manifest")]
public class ManifestController(ManagementServerContext dataContext) : ControllerBase
{
    private ManagementServerContext DataContext { get; } = dataContext;

    [HttpGet("get/{cuid}")]
    public IActionResult GetManifest([FromRoute] string cuid)
    {
        if (!DataContext.Clients.Any(i => i.Cuid == cuid))
        {
            return BadRequest("实例不存在。");
        }

        var updates = DataContext.ObjectUpdates.Where(i => i.TargetCuid == cuid).Select(i => i);
        

        return Ok(new ManagementManifest());
    }
    
}