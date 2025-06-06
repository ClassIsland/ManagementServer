using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.ManagementServer.Server.Enums;
using ClassIsland.ManagementServer.Server.Extensions;
using ClassIsland.ManagementServer.Server.Models;
using ClassIsland.ManagementServer.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Controllers.Profiles;

[ApiController]
[Authorize]
[Route("api/v1/profiles/subjects")]
public class SubjectsController(ManagementServerContext dbContext, 
    ObjectsCacheService objectsCacheService) : ControllerBase
{
    public ManagementServerContext DbContext { get; } = dbContext;
    public ObjectsCacheService ObjectsCacheService { get; } = objectsCacheService;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
    {
        return Ok(await DbContext.ProfileSubjects.Include(x => x.Group)
            .Select(x => x).ToPaginatedListAsync(pageIndex, pageSize));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Put([FromBody] ProfileSubject payload, [FromRoute] Guid id)
    {
        var prev = await DbContext.ProfileSubjects.AnyAsync(x => x.Id == id);
        if (!prev)
        {
            return NotFound(new Error("找不到请求的对象"));
        }
        DbContext.Entry(payload).State = EntityState.Modified;
        await DbContext.SaveChangesAsync();
        ObjectsCacheService.InvalidateCache(ObjectTypes.ProfileSubject, id);
        return Ok();
    }
    
    [HttpPut]
    public async Task<IActionResult> Put([FromBody] ProfileSubject payload)
    {
        payload.Id = new Guid();
        await DbContext.ProfileSubjects.AddAsync(payload);
        await DbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var entity = await DbContext.ProfileSubjects.FindAsync(id);
        if (entity == null) 
        {
            return NotFound(new Error("找不到请求的对象"));
        }

        DbContext.ProfileSubjects.Remove(entity);
        await DbContext.SaveChangesAsync();
        ObjectsCacheService.InvalidateCache(ObjectTypes.ProfileSubject, id);
        return Ok();
    }
}