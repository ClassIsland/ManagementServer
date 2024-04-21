using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
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
    
    [HttpGet("{id:int}")]
    public IActionResult Get([FromRoute] int id)
    {
        var v = DbContext.ObjectsAssignees.FirstOrDefault(x => x.Id == id);
        if (v == null)
            return NotFound();
        return Ok(v);
    }
    
    [HttpPost("{id:int}")]
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
    
    [HttpPost]
    public async Task<IActionResult> Update([FromBody] ObjectsAssignee content)
    {
        DbContext.ObjectsAssignees.Add(content);
        await ObjectsUpdateNotifyService.NotifyClientUpdatingAsync(content.TargetClientCuid, content.TargetClientId,
            content.TargetGroupId);
        await DbContext.SaveChangesAsync();
        return Ok(content);
    }
    
    [HttpDelete("{id:int}")]
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
}