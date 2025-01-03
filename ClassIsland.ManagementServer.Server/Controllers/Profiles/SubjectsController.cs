using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.ManagementServer.Server.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Controllers.Profiles;

[ApiController]
[Route("api/v1/profiles/subjects")]
public class SubjectsController(ManagementServerContext dbContext) : ControllerBase
{
    public ManagementServerContext DbContext { get; } = dbContext;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
    {
        return Ok(await DbContext.ProfileSubjects.Select(x => x).ToPaginatedListAsync(pageIndex, pageSize));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Put([FromBody] ProfileSubject payload, [FromRoute] Guid id)
    {
        var prev = await DbContext.ProfileSubjects.AnyAsync(x => x.Id == id);
        if (!prev)
        {
            return BadRequest();
        }
        DbContext.Entry(payload).State = EntityState.Modified;
        await DbContext.SaveChangesAsync();
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
        var entity = await DbContext.ProfileSubjects.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) 
        {
            return NotFound();
        }

        DbContext.ProfileSubjects.Remove(entity);
        await DbContext.SaveChangesAsync();
        return Ok();
    }
}