using System.Collections.ObjectModel;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using ClassIsland.Core;
using ClassIsland.Core.Models.Management;
using ClassIsland.Core.Models.Profile;
using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
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
    public async Task<IActionResult> GetSubjects([FromRoute] string cuid)
    {
        var client = DbContext.Clients.FirstOrDefault(x => x.Cuid == cuid);
        if (client == null)
            return NotFound();
        var assignees = await ObjectsAssigneeService.GetClientAssigningObjects(client, ObjectTypes.ProfileSubject);
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
                IsOutDoor = subject.IsOutDoor ?? false,
                TeacherName = "",
                AttachedObjects = JsonSerializer.Deserialize<Dictionary<string, object?>>(subject.AttachedObjects ?? "{}", JsonOptions) ?? new()
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
    public async Task<IActionResult> GetTimeLayouts([FromRoute] string cuid)
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
                        StartSecond = ConvertTimeOnly(x.Start ?? TimeOnly.MinValue),
                        EndSecond = ConvertTimeOnly(x.End ?? TimeOnly.MinValue),
                        TimeType = x.TimeType ?? 0,
                        DefaultClassId = x.DefaultSubjectId ?? "",
                        IsHideDefault = x.IsHideDefault ?? false,
                        AttachedObjects =
                            JsonSerializer.Deserialize<Dictionary<string, object?>>(x.AttachedObjects ?? "{}",
                                JsonOptions) ?? new()
                    }
                ));
            timeLayouts.Add(tl.Id, new TimeLayout()
            {
                Name = tl.Name ?? "",
                Layouts = tp,
                AttachedObjects = JsonSerializer.Deserialize<Dictionary<string, object?>>(tl.AttachedObjects ?? "{}",
                    JsonOptions) ?? new()
            });
        }

        return Ok(new Profile()
        {
            TimeLayouts = timeLayouts
        });
    }
    
    [HttpGet("classplans")]
    public async Task<IActionResult> GetClassPlans([FromRoute] string cuid)
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
                        SubjectId = x.SubjectId,
                        // TODO: AttachedObjects =
                        //     JsonSerializer.Deserialize<Dictionary<string, object?>>(x.AttachedObjects ?? "{}",
                        //         JsonOptions) ?? new()
                    }
                ));
            classPlans.Add(cp.Id, new ClassPlan()
            {
                Name = cp.Name ?? "",
                TimeRule = new TimeRule()
                {
                    WeekDay = cp.WeekDay ?? 0,
                    WeekCountDiv = cp.WeekDiv ?? 0
                },
                TimeLayoutId = cp.TimeLayoutId ?? "",
                Classes = c,
                AttachedObjects = JsonSerializer.Deserialize<Dictionary<string, object?>>(cp.AttachedObjects ?? "{}",
                    JsonOptions) ?? new()
            });
        }

        return Ok(new Profile()
        {
            ClassPlans = classPlans
        });
    }

    [HttpGet("policy")]
    public async Task<IActionResult> GetPolicy([FromRoute] string cuid)
    {
        var client = DbContext.Clients.FirstOrDefault(x => x.Cuid == cuid);
        if (client == null)
            return NotFound();
        // TODO: 按照分配粒度分配策略
        var assignees = await ObjectsAssigneeService.GetClientAssigningObjects(client, ObjectTypes.Policy);
        foreach (var policy in assignees.Select(i => DbContext.Policies.FirstOrDefault(x => x.Id == i.ObjectId)).OfType<Policy>().Where(policy => (policy.IsEnabled ?? false)))
        {
            return Ok(policy.Content ?? "{}");
        }
        return Ok(new ManagementPolicy());
    }
}