using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Controllers;

[ApiController]
[Route("api/v1/policies")]
public class PoliciesController(ManagementServerContext dbContext) : ControllerBase
{
    private ManagementServerContext DbContext { get; } = dbContext;

    [HttpGet]
    public async Task<IActionResult> List()
    {
        return Ok(await DbContext.Policies.Select(i => i).ToListAsync());
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var o = await DbContext.Policies.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (o == null)
        {
            return NotFound();
        }

        return Ok(o);
    }
    
    [HttpPost("{id}")]
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
    public async Task<IActionResult> Update([FromBody] Policy policy)
    {
        policy.Id = Guid.NewGuid();
        await DbContext.Policies.AddAsync(policy);
        await DbContext.SaveChangesAsync();
        return Ok(policy);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var o = await DbContext.Policies.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (o == null)
        {
            return NotFound();
        }

        DbContext.Policies.Remove(o);
        await DbContext.SaveChangesAsync();

        return Ok(o);
    }
}