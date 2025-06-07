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
[Route("api/v1/profiles/classPlans")]
public class ClassPlansController(ManagementServerContext dbContext, 
    ProfileEntitiesService profileEntitiesService) : ControllerBase
{
    public ManagementServerContext DbContext { get; } = dbContext;
    public ProfileEntitiesService ProfileEntitiesService { get; } = profileEntitiesService;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
    {
        return Ok(await DbContext.ProfileClassplans.Include(x => x.Group).Select(x => x).ToPaginatedListAsync(pageIndex, pageSize));
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var v = await ProfileEntitiesService.GetClassPlanEntity(id);
        if (v == null)
        {
            return NotFound(new Error("找不到请求的对象"));
        }
        return Ok(new
        {
            ClassPlan = v,
            ClassNames = await DbContext.ProfileClassplanClasses
                .Where(x => x.ParentId == id)
                .OrderBy(x => x.Index)
                .Select(x => x.Subject.Name)
                .ToListAsync()
        });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.ObjectsWrite)]
    public async Task<IActionResult> Put([FromBody] ClassPlan payload, [FromRoute] Guid id)
    {
        var prev = await DbContext.ProfileClassplans.FirstOrDefaultAsync(x => x.Id == id);
        if (prev == null)
        {
            return NotFound(new Error("找不到请求的对象"));
        }
        
        await ProfileEntitiesService.SetClassPlanEntity(id, payload, true);
        await DbContext.SaveChangesAsync();
        return Ok();
    }
    
    [HttpPut]
    [Authorize(Roles = Roles.ObjectsWrite)]
    public async Task<IActionResult> Put([FromBody] ClassPlan payload)
    {
        var id = new Guid();
        await ProfileEntitiesService.SetClassPlanEntity(id, payload, false);
        await DbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.ObjectsDelete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var entity = await DbContext.ProfileClassplans.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) 
        {
            return NotFound(new Error("找不到请求的对象"));
        }

        DbContext.ProfileClassplans.Remove(entity);
        await DbContext.SaveChangesAsync();
        return Ok();
    }
}