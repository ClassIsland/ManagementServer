using ClassIsland.ManagementServer.Server.Authorization;
using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.ManagementServer.Server.Extensions;
using ClassIsland.ManagementServer.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/policies")]
public class PoliciesController(ManagementServerContext dbContext) : ControllerBase
{
    private ManagementServerContext DbContext { get; } = dbContext;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
    {
        return Ok(await DbContext.Policies.Select(i => i)
            .Include(x => x.Group)
            .ToPaginatedListAsync(pageIndex, pageSize));
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var o = await DbContext.Policies.Where(x => x.Id == id)
            .Include(x => x.Group)
            .FirstOrDefaultAsync();
        if (o == null)
        {
            return NotFound(new Error("找不到请求的对象"));
        }

        return Ok(o);
    }
    
    [HttpPost("{id}")]
    [Authorize(Roles = Roles.ObjectsWrite)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] Policy policy)
    {
        var o = await DbContext.Policies.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();
        policy.Id = id;
        if (o == null)
        {
            await DbContext.Policies.AddAsync(policy);
        }
        else
        {
            DbContext.Policies.Entry(policy).State = EntityState.Modified;
            policy.CreatedTime = o.CreatedTime;
        }

        await DbContext.SaveChangesAsync();
        return Ok(policy);
    }
    
    [HttpPost]
    [Authorize(Roles = Roles.ObjectsWrite)]
    public async Task<IActionResult> Update([FromBody] Policy policy)
    {
        policy.Id = Guid.NewGuid();
        await DbContext.Policies.AddAsync(policy);
        await DbContext.SaveChangesAsync();
        return Ok(policy);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.ObjectsDelete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var o = await DbContext.Policies.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (o == null)
        {
            return NotFound(new Error("找不到请求的对象"));
        }

        DbContext.Policies.Remove(o);
        await DbContext.SaveChangesAsync();

        return Ok(o);
    }
}