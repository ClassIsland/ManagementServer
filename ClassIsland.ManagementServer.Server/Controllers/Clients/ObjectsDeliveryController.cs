using System.Collections.ObjectModel;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.ManagementServer.Server.Enums;
using ClassIsland.ManagementServer.Server.Services;
using ClassIsland.Shared;
using ClassIsland.Shared.Models.Management;
using ClassIsland.Shared.Models.Profile;
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
    public async Task<IActionResult> GetSubjects([FromRoute] Guid cuid)
    {
        var client = DbContext.Clients.FirstOrDefault(x => x.Cuid == cuid);
        if (client == null)
            return NotFound();
        var assignees = await ObjectsAssigneeService.GetClientAssigningObjects(client, ObjectTypes.ProfileSubject);
        var subjects = new ObservableDictionary<string, Subject>();
        foreach (var subject in assignees.Select(i => DbContext.ProfileSubjects.FirstOrDefault(x => x.Id == i.ObjectId)).OfType<ProfileSubject>())
        {
            subjects.Add(subject.Id.ToString(), new Subject()
            {
                Name = subject.Name,
                Initial = subject.Initials,
                IsOutDoor = subject.IsOutDoor,
                TeacherName = "",
                AttachedObjects = subject.AttachedObjects
            });
        }

        return Ok(new Profile()
        {
            Subjects = subjects
        });
    }

    private DateTime ConvertTimeOnly(TimeOnly t)
    {
        var now = DateTime.Now;
        return new DateTime(now.Year, now.Month, now.Day, t.Hour, t.Minute, t.Second);
    }

    [HttpGet("timelayouts")]
    public async Task<IActionResult> GetTimeLayouts([FromRoute] Guid cuid)
    {
        var client = DbContext.Clients.FirstOrDefault(x => x.Cuid == cuid);
        if (client == null)
            return NotFound();
        var assignees = await ObjectsAssigneeService.GetClientAssigningObjects(client, ObjectTypes.ProfileTimeLayout);
        var timeLayouts = new ObservableDictionary<string, TimeLayout>();
        foreach (var i in assignees)
        {
            var tl = DbContext.ProfileTimelayouts.FirstOrDefault(x => x.Id == i.ObjectId);
            if (tl == null)
                continue;
            var tp = new ObservableCollection<TimeLayoutItem>(DbContext.ProfileTimelayoutTimepoints
                .Where(x => x.ParentId == tl.Id).OrderBy(x => x.Index).Select(x => x).ToList()
                .Select(x =>
                    new TimeLayoutItem()
                    {
                        StartSecond = ConvertTimeOnly(x.Start),
                        EndSecond = ConvertTimeOnly(x.End),
                        TimeType = x.TimeType,
                        DefaultClassId = x.DefaultSubjectId ?? "",
                        IsHideDefault = x.IsHideDefault,
                        AttachedObjects = x.AttachedObjects
                    }
                ));
            timeLayouts.Add(tl.Id.ToString(), new TimeLayout()
            {
                Name = tl.Name,
                Layouts = tp,
                AttachedObjects = tl.AttachedObjects
            });
        }

        return Ok(new Profile()
        {
            TimeLayouts = timeLayouts
        });
    }
    
    [HttpGet("classplans")]
    public async Task<IActionResult> GetClassPlans([FromRoute] Guid cuid)
    {
        var client = DbContext.Clients.FirstOrDefault(x => x.Cuid == cuid);
        if (client == null)
            return NotFound();
        var assignees = await ObjectsAssigneeService.GetClientAssigningObjects(client, ObjectTypes.ProfileClassPlan);
        var classPlans = new ObservableDictionary<string, ClassPlan>();
        foreach (var i in assignees)
        {
            var cp = DbContext.ProfileClassplans.FirstOrDefault(x => x.Id == i.ObjectId);
            if (cp == null)
                continue;
            var c = new ObservableCollection<ClassInfo>(DbContext.ProfileClassplanClasses    
                .Where(x => x.ParentId == cp.Id).OrderBy(x => x.Index).Select(x => x).ToList()
                .Select(x =>
                    new ClassInfo()
                    {
                        SubjectId = x.SubjectId.ToString(),
                        // TODO: AttachedObjects =
                        //     JsonSerializer.Deserialize<Dictionary<string, object?>>(x.AttachedObjects ?? "{}",
                        //         JsonOptions) ?? new()
                    }
                ));
            classPlans.Add(cp.Id.ToString(), new ClassPlan()
            {
                Name = cp.Name,
                TimeRule = new TimeRule()
                {
                    WeekDay = cp.WeekDay,
                    WeekCountDiv = cp.WeekDiv
                },
                TimeLayoutId = cp.TimeLayoutId.ToString(),
                Classes = c,
                AttachedObjects = cp.AttachedObjects
            });
        }

        return Ok(new Profile()
        {
            ClassPlans = classPlans
        });
    }

    [HttpGet("policy")]
    public async Task<IActionResult> GetPolicy([FromRoute] Guid cuid)
    {
        var client = DbContext.Clients.FirstOrDefault(x => x.Cuid == cuid);
        if (client == null)
            return NotFound();
        var assignees = await ObjectsAssigneeService.GetClientAssigningObjectsLeveled(client, ObjectTypes.Policy);
        foreach (var policy in assignees.Select(i => DbContext.Policies.FirstOrDefault(x => x.Id == i.ObjectId)).OfType<Policy>().Where(policy => (policy.IsEnabled)))
        {
            // todo
            return Ok(policy.Content);
        }
        return Ok(new ManagementPolicy());
    }
    
    [HttpGet("default-settings")]
    public async Task<IActionResult> GetDefaultSettings([FromRoute] Guid cuid)
    {
        var client = DbContext.Clients.FirstOrDefault(x => x.Cuid == cuid);
        if (client == null)
            return NotFound();
        var assignees = await ObjectsAssigneeService.GetClientAssigningObjectsLeveled(client, ObjectTypes.AppSettings);
        foreach (var setting in assignees.Select(i => DbContext.Settings.FirstOrDefault(x => x.Id == i.ObjectId)).OfType<Setting>())
        {
            return Ok(setting.Settings ?? "{}");
        }
        return Ok(new ManagementPolicy());
    }
}