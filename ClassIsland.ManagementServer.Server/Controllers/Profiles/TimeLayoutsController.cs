using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.ManagementServer.Server.Extensions;
using ClassIsland.ManagementServer.Server.Services;
using ClassIsland.Shared.Models.Profile;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Controllers.Profiles;

[ApiController]
[Route("api/v1/profiles/timeLayouts")]
public class TimeLayoutsController(ManagementServerContext dbContext, ProfileEntitiesService profileEntitiesService) : ControllerBase
{
    public ManagementServerContext DbContext { get; } = dbContext;
    public ProfileEntitiesService ProfileEntitiesService { get; } = profileEntitiesService;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
    {
        return Ok(await DbContext.ProfileTimelayouts.Include(x => x.Group).Select(x => x).ToPaginatedListAsync(pageIndex, pageSize));
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var v = await ProfileEntitiesService.GetTimeLayoutEntity(id);
        if (v == null)
        {
            return NotFound();
        }
        return Ok(v);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Put([FromBody] TimeLayout payload, [FromRoute] Guid id)
    {
        var prev = await DbContext.ProfileTimelayouts.FirstOrDefaultAsync(x => x.Id == id);
        if (prev == null)
        {
            return NotFound();
        }
        
        await ProfileEntitiesService.SetTimeLayoutEntity(id, payload, true);
        await DbContext.SaveChangesAsync();
        return Ok();
    }
    
    [HttpPut]
    public async Task<IActionResult> Put([FromBody] TimeLayout payload)
    {
        var id = new Guid();
        await ProfileEntitiesService.SetTimeLayoutEntity(id, payload, false);
        await DbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var entity = await DbContext.ProfileTimelayouts.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) 
        {
            return NotFound();
        }

        DbContext.ProfileTimelayouts.Remove(entity);
        await DbContext.SaveChangesAsync();
        return Ok();
    }
}