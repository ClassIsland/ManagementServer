using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Controllers;

[ApiController]
[Route("api/v1/assignees/{id:int}")]
public class AssigneesController(ManagementServerContext dbContext) : ControllerBase
{
    private ManagementServerContext DbContext { get; } = dbContext;
    
    [HttpGet]
    public IActionResult Get([FromRoute] int id)
    {
        var v = DbContext.ObjectsAssignees.FirstOrDefault(x => x.Id == id);
        if (v == null)
            return NotFound();
        return Ok(v);
    }
    
    [HttpPost]
    public IActionResult Update([FromBody] ObjectsAssignee content, [FromRoute] int? id = null)
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

        DbContext.SaveChanges();
        return Ok(content);
    }
    
    [HttpDelete]
    public IActionResult Delete([FromRoute] int id)
    {
        var v = DbContext.ObjectsAssignees.FirstOrDefault(x => x.Id == id);
        if (v == null)
            return NotFound();
        DbContext.ObjectsAssignees.Remove(v);
        return Ok();
    }
}