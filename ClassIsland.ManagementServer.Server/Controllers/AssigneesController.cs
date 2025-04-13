using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.ManagementServer.Server.Extensions;
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
        if (DbContext.ObjectsAssignees.Any(x => x.Id == id) && id != null)
        {
            content.Id = id.Value;
            DbContext.Entry(content).State = EntityState.Modified;
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
    
    [HttpGet("by-object/{objectType:int}/{id:guid}")]
    public async Task<IActionResult> GetByObject([FromRoute] int objectType, [FromRoute] Guid id, 
        [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
    {
        var assignees = await DbContext.ObjectsAssignees
            .Where(x => (int)x.ObjectType == objectType && x.ObjectId == id)
            .OrderBy(x => x.Id)
            .ToPaginatedListAsync(pageIndex, pageSize);
        return Ok(assignees);
    }
}