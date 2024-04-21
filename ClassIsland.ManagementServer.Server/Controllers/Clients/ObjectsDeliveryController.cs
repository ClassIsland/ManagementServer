using System.Text.Json;
using ClassIsland.Core;
using ClassIsland.Core.Models.Profile;
using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Enums;
using ClassIsland.ManagementServer.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClassIsland.ManagementServer.Server.Controllers.Clients;

[ApiController]
[Route("api/v1/client/{cuid}/")]
public class ObjectsDeliveryController(
    ManagementServerContext dataContext,
    ObjectsUpdateNotifyService objectsUpdateNotifyService,
    ObjectsAssigneeService objectsAssigneeService) : ControllerBase
{
    private ManagementServerContext DbContext { get; } = dataContext;
    
    private ObjectsUpdateNotifyService ObjectsUpdateNotifyService { get; } = objectsUpdateNotifyService;
    
    private ObjectsAssigneeService ObjectsAssigneeService { get; } = objectsAssigneeService;

    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
    
    [HttpGet("subjects")]
    public IActionResult GetSubjects([FromRoute] string cuid)
    {
        var client = DbContext.Clients.FirstOrDefault(x => x.Cuid == cuid);
        if (client == null)
            return NotFound();
        var assignees = DbContext.ObjectsAssignees.Where(x => 
            x.ObjectType == (int)ObjectTypes.ProfileSubject &&
            ((x.TargetClientCuid != null && x.TargetClientCuid == cuid) ||
             (x.TargetClientId != null && x.TargetClientId == client.Id) ||
             (x.TargetGroupId != null && x.TargetGroupId == client.GroupId))).Select(x => x).ToList();
        var subjects = new ObservableDictionary<string, Subject>();
        foreach (var i in assignees)
        {
            var subject = DbContext.ProfileSubjects.FirstOrDefault(x => x.Id == i.ObjectId);
            if (subject == null)
                continue;
            subjects.Add(subject.Id, new Subject()
            {
                Name = subject.Name ?? "",
                Initial = subject.Initials ?? "",
                // TODO: IsOutDoor = ...,
                TeacherName = "",
                AttachedObjects = JsonSerializer.Deserialize<Dictionary<string, object?>>(subject.AttachedObjects ?? "{}", JsonOptions) ?? new()
            });
        }

        return Ok(new Profile()
        {
            Subjects = subjects
        });
    }
}