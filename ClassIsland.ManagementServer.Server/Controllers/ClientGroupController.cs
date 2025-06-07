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
            return NotFound(new Error("找不到请求的对象"));
        }
        
        return Ok(group);
    }
    
    [HttpPut]
    [Authorize(Roles = Roles.ClientsWrite)]
    public async Task<IActionResult> CreateGroup([FromBody] ClientGroup group)
    {
        await DbContext.ClientGroups.AddAsync(group);
        await DbContext.SaveChangesAsync();
        
        return Ok(group);
    }
    
    [HttpPut("{id:long}")]
    [Authorize(Roles = Roles.ClientsWrite)]
    public async Task<IActionResult> UpdateGroup(long id, [FromBody] ClientGroup group)
    {   
        var prev = await DbContext.ClientGroups.AnyAsync(x => x.Id == id);
        if (!prev)
        {
            return NotFound(new Error("找不到请求的对象"));
        }
        DbContext.Entry(group).State = EntityState.Modified;
        await DbContext.SaveChangesAsync();
        return Ok();
    }
    
    [HttpDelete("{id:long}")]
    [Authorize(Roles = Roles.ClientsDelete)]
    public async Task<IActionResult> DeleteGroup(long id)
    {
        var group = await DbContext.ClientGroups.FirstOrDefaultAsync(x => x.Id == id);
        if (group == null) 
        {
            return NotFound(new Error("找不到请求的对象"));
        }

        if (id is ClientGroup.DefaultGroupId or ClientGroup.GlobalGroupId)
        {
            return BadRequest(new Error("不能删除默认组或全局分组"));
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