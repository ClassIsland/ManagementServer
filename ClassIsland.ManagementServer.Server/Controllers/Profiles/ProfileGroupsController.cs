using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.ManagementServer.Server.Extensions;
using ClassIsland.ManagementServer.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Controllers.Profiles;

[ApiController]
[Authorize]
[Route("api/v1/profiles/groups")]
public class ProfileGroupsController(ManagementServerContext dbContext) : ControllerBase
{
    public ManagementServerContext DbContext { get; } = dbContext;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
    {
        return Ok(await DbContext.ProfileGroups
            .Select(x => x)
            .ToPaginatedListAsync(pageIndex, pageSize));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Put([FromBody] ProfileGroup payload, [FromRoute] Guid id)
    {
        var prev = await DbContext.ProfileGroups.AnyAsync(x => x.Id == id);
        if (!prev)
        {
            return NotFound(new Error("找不到请求的对象"));
        }
        DbContext.Entry(payload).State = EntityState.Modified;
        await DbContext.SaveChangesAsync();
        return Ok();
    }
    
    [HttpPut]
    public async Task<IActionResult> Put([FromBody] ProfileGroup payload)
    {
        payload.Id = new Guid();
        await DbContext.ProfileGroups.AddAsync(payload);
        await DbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var entity = await DbContext.ProfileGroups.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) 
        {
            return NotFound(new Error("找不到请求的对象"));
        }

        DbContext.ProfileGroups.Remove(entity);
        await DbContext.SaveChangesAsync();
        return Ok();
    }
}