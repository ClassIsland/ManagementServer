using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.ManagementServer.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/default-settings")]
public class AppSettingsController(ManagementServerContext dbContext) : ControllerBase
{
    private ManagementServerContext DbContext { get; } = dbContext;
    
    [HttpGet]
    public async Task<IActionResult> List()
    {
        return Ok(await DbContext.Settings.Select(i => i).ToListAsync());
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var o = await DbContext.Settings.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (o == null)
        {
            return NotFound(new Error("找不到请求的对象"));
        }

        return Ok(o);
    }
    
    [HttpPost("{id}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] Setting setting)
    {
        var o = await DbContext.Settings.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();
        setting.Id = id;
        if (o == null)
        {
            await DbContext.Settings.AddAsync(setting);
        }
        else
        {
            DbContext.Settings.Entry(setting).State = EntityState.Modified;
            setting.CreatedTime = o.CreatedTime;
        }

        await DbContext.SaveChangesAsync();
        return Ok(setting);
    }
    
    [HttpPost]
    public async Task<IActionResult> Update([FromBody] Setting setting)
    {
        setting.Id = Guid.NewGuid();
        await DbContext.Settings.AddAsync(setting);
        await DbContext.SaveChangesAsync();
        return Ok(setting);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var o = await DbContext.Settings.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (o == null)
        {
            return NotFound(new Error("找不到请求的对象"));
        }

        DbContext.Settings.Remove(o);
        await DbContext.SaveChangesAsync();

        return Ok(o);
    }
}