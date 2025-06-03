using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.ManagementServer.Server.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/client_groups")]
public class ClientGroupController(ILogger<ClientGroupController> logger, ManagementServerContext dbContext) : Controller
{
    public ILogger<ClientGroupController> Logger { get; } = logger;
    public ManagementServerContext DbContext { get; } = dbContext;
    
    [HttpGet]
    public async Task<IActionResult> GetAllGroups([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
    {
        var groups = await DbContext.ClientGroups
            .Select(x => x)
            .ToPaginatedListAsync(pageIndex, pageSize);
        
        return Ok(groups);
    }
    
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetGroupById(long id)
    {
        var group = await DbContext.ClientGroups
            .FirstOrDefaultAsync(x => x.Id == id);
        
        if (group == null)
        {
            return NotFound();
        }
        
        return Ok(group);
    }
    
    [HttpPut]
    public async Task<IActionResult> CreateGroup([FromBody] ClientGroup group)
    {
        await DbContext.ClientGroups.AddAsync(group);
        await DbContext.SaveChangesAsync();
        
        return Ok(group);
    }
    
    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateGroup(long id, [FromBody] ClientGroup group)
    {   
        var prev = await DbContext.ClientGroups.AnyAsync(x => x.Id == id);
        if (!prev)
        {
            return NotFound();
        }
        DbContext.Entry(group).State = EntityState.Modified;
        await DbContext.SaveChangesAsync();
        return Ok();
    }
    
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteGroup(long id)
    {
        var group = await DbContext.ClientGroups.FirstOrDefaultAsync(x => x.Id == id);
        if (group == null) 
        {
            return NotFound();
        }

        if (id is ClientGroup.DefaultGroupId or ClientGroup.GlobalGroupId)
        {
            return BadRequest();
        }

        var clients = await DbContext.AbstractClients
            .Where(x => x.GroupId == id)
            .ToListAsync();
        foreach (var i in clients)
        {
            i.GroupId = ClientGroup.DefaultGroupId;
        }
        
        DbContext.ClientGroups.Remove(group);
        await DbContext.SaveChangesAsync();
        return Ok();
    }
}