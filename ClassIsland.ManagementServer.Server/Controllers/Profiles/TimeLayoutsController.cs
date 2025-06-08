using ClassIsland.ManagementServer.Server.Authorization;
using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.ManagementServer.Server.Extensions;
using ClassIsland.ManagementServer.Server.Models;
using ClassIsland.ManagementServer.Server.Services;
using ClassIsland.Shared.Models.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Controllers.Profiles;

[ApiController]
[Authorize]
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
            return NotFound(new Error("找不到请求的对象"));
        }
        return Ok(v);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.ObjectsWrite)]
    public async Task<IActionResult> Put([FromBody] TimeLayout payload, [FromRoute] Guid id)
    {
        var prev = await DbContext.ProfileTimelayouts.FirstOrDefaultAsync(x => x.Id == id);
        if (prev == null)
        {
            return NotFound(new Error("找不到请求的对象"));
        }
        
        await ProfileEntitiesService.SetTimeLayoutEntity(id, payload, true);
        await DbContext.SaveChangesAsync();
        return Ok();
    }
    
    [HttpPut]
    [Authorize(Roles = Roles.ObjectsWrite)]
    public async Task<IActionResult> Put([FromBody] TimeLayout payload)
    {
        var id = new Guid();
        await ProfileEntitiesService.SetTimeLayoutEntity(id, payload, false);
        await DbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.ObjectsDelete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var entity = await DbContext.ProfileTimelayouts.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) 
        {
            return NotFound(new Error("找不到请求的对象"));
        }

        if (await DbContext.ProfileClassplans.AnyAsync(x => x.TimeLayoutId == id))
        {
            return BadRequest(new Error("不能删除此时间表，因为有课表引用了此时间表"));
        }

        DbContext.ProfileTimelayouts.Remove(entity);
        await DbContext.SaveChangesAsync();
        return Ok();
    }
}