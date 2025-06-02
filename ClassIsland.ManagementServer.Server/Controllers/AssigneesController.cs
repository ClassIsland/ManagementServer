using System.Collections.ObjectModel;
using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.ManagementServer.Server.Enums;
using ClassIsland.ManagementServer.Server.Extensions;
using ClassIsland.ManagementServer.Server.Models.Assignees;
using ClassIsland.ManagementServer.Server.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Controllers;

[ApiController]
[Route("api/v1/assignees/")]
public class AssigneesController(ManagementServerContext dbContext, 
    ObjectsAssigneeService objectsAssigneeService,
    ObjectsUpdateNotifyService objectsUpdateNotifyService) : ControllerBase
{
    private ObjectsAssigneeService ObjectsAssigneeService { get; } = objectsAssigneeService;

    private ObjectsUpdateNotifyService ObjectsUpdateNotifyService { get; } = objectsUpdateNotifyService;
    
    private ManagementServerContext DbContext { get; } = dbContext;
    
    [HttpGet("all/{id:int}")]
    public IActionResult GetAll([FromRoute] int id)
    {
        var v = DbContext.ObjectsAssignees.FirstOrDefault(x => x.Id == id);
        if (v == null)
            return NotFound();
        return Ok(v);
    }
    
    [HttpPost("all/{id:int}")]
    public async Task<IActionResult> Update([FromBody] ObjectsAssignee content, [FromRoute] int? id = null)
    {
        var raw = await DbContext.ObjectsAssignees.FirstOrDefaultAsync(x => x.Id == id);
        if (raw != null && id != null)
        {
            content.Id = id.Value;
            DbContext.Entry(content).State = EntityState.Modified;
            content.CreatedTime = raw.CreatedTime;
        }
        else
        {
            DbContext.ObjectsAssignees.Add(content);
        }
        await ObjectsUpdateNotifyService.NotifyClientUpdatingAsync(content.TargetClientCuid, content.TargetClientId,
            content.TargetGroupId);
        await DbContext.SaveChangesAsync();
        return Ok(content);
    }
    
    [HttpPost("all")]
    public async Task<IActionResult> Update([FromBody] ObjectsAssignee content)
    {
        DbContext.ObjectsAssignees.Add(content);
        await DbContext.SaveChangesAsync();
        await ObjectsUpdateNotifyService.NotifyClientUpdatingAsync(content.TargetClientCuid, content.TargetClientId,
            content.TargetGroupId);
        await DbContext.SaveChangesAsync();
        return Ok(content);
    }
    
    [HttpDelete("all/{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var v = DbContext.ObjectsAssignees.FirstOrDefault(x => x.Id == id);
        if (v == null)
            return NotFound();
        await ObjectsUpdateNotifyService.NotifyClientUpdatingAsync(v.TargetClientCuid, v.TargetClientId,
            v.TargetGroupId);
        DbContext.ObjectsAssignees.Remove(v);
        await DbContext.SaveChangesAsync();
        return Ok();
    }
    
    [HttpGet("by-object/assignees/{objectType:int}/{id:guid}")]
    public async Task<IActionResult> GetByObject([FromRoute] int objectType, [FromRoute] Guid id, 
        [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
    {
        var assignees = await DbContext.ObjectsAssignees
            .Where(x => (int)x.ObjectType == objectType && x.ObjectId == id)
            .OrderBy(x => x.Id)
            .ToPaginatedListAsync(pageIndex, pageSize);
        return Ok(assignees);
    }
    
    [HttpGet("by-object/clients/{objectType:int}/{id:guid}")]
    public async Task<IActionResult> GetByObjectInClientView([FromRoute] int objectType, [FromRoute] Guid id,
        [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
    {
        var clients = await DbContext.Clients
            .ToPaginatedListAsync(pageIndex, pageSize);
        Collection<ClientAssigneeState<Client>> states = [];
        foreach (var client in clients.Items)
        {
            var assignee = await DbContext.ObjectsAssignees.FirstOrDefaultAsync(x =>
                x.AssigneeType == AssigneeTypes.ClientUid && x.TargetClientCuid == client.Cuid &&
                (int)x.ObjectType == objectType && x.ObjectId == id);
            states.Add(new ClientAssigneeState<Client>(AssigneeTypes.ClientUid, client, assignee));
        }
        
        return Ok(states.ToPaginatedList(pageIndex, pageSize));
    }
    
    [HttpGet("by-object/clients-abstract/{objectType:int}/{id:guid}")]
    public async Task<IActionResult> GetByObjectInAbstractClientView([FromRoute] int objectType, [FromRoute] Guid id,
        [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
    {
        var clients = await DbContext.AbstractClients
            .Include(x => x.Group)
            .ToPaginatedListAsync(pageIndex, pageSize);
        Collection<ClientAssigneeState<AbstractClient>> states = [];
        foreach (var client in clients.Items)
        {
            var assignee = await DbContext.ObjectsAssignees.FirstOrDefaultAsync(x =>
                x.AssigneeType == AssigneeTypes.Id && x.TargetClientId == client.Id &&
                (int)x.ObjectType == objectType && x.ObjectId == id);
            states.Add(new ClientAssigneeState<AbstractClient>(AssigneeTypes.ClientUid, client, assignee));
        }
        
        return Ok(states.ToPaginatedList(pageIndex, pageSize));
    }
    
    [HttpGet("by-object/client-groups/{objectType:int}/{id:guid}")]
    public async Task<IActionResult> GetByObjectInClientGroupsView([FromRoute] int objectType, [FromRoute] Guid id,
        [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
    {
        var clientGroups = await DbContext.ClientGroups
            .ToPaginatedListAsync(pageIndex, pageSize);
        Collection<ClientAssigneeState<ClientGroup>> states = [];
        foreach (var group in clientGroups.Items)
        {
            var assignee = await DbContext.ObjectsAssignees.FirstOrDefaultAsync(x =>
                x.AssigneeType == AssigneeTypes.Group && x.TargetGroupId == group.Id &&
                (int)x.ObjectType == objectType && x.ObjectId == id);
            states.Add(new ClientAssigneeState<ClientGroup>(AssigneeTypes.ClientUid, group, assignee));
        }
        
        return Ok(states.ToPaginatedList(pageIndex, pageSize));
    }
}