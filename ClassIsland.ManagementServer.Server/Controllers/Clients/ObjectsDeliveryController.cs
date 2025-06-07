using System.Collections.ObjectModel;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.ManagementServer.Server.Enums;
using ClassIsland.ManagementServer.Server.Models;
using ClassIsland.ManagementServer.Server.Services;
using ClassIsland.Shared;
using ClassIsland.Shared.Models.Management;
using ClassIsland.Shared.Models.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Controllers.Clients;

[AllowAnonymous]
[ApiController]
[Route("api/v1/client/{cuid}/")]
public class ObjectsDeliveryController(
    ManagementServerContext dataContext,
    ObjectsUpdateNotifyService objectsUpdateNotifyService,
    ObjectsAssigneeService objectsAssigneeService,
    ProfileEntitiesService profileEntitiesService) : ControllerBase
{
    private ManagementServerContext DbContext { get; } = dataContext;
    
    private ObjectsUpdateNotifyService ObjectsUpdateNotifyService { get; } = objectsUpdateNotifyService;
    
    private ObjectsAssigneeService ObjectsAssigneeService { get; } = objectsAssigneeService;
    public ProfileEntitiesService ProfileEntitiesService { get; } = profileEntitiesService;

    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
    
    [HttpGet("subjects")]
    public async Task<IActionResult> GetSubjects([FromRoute] Guid cuid)
    {
        var client = DbContext.Clients.FirstOrDefault(x => x.Cuid == cuid);
        if (client == null)
            return NotFound(new Error("找不到请求的对象"));
        var assignees = await ObjectsAssigneeService.GetClientAssigningObjects(client, ObjectTypes.ProfileSubject);
        var subjects = new ObservableDictionary<string, Subject>();
        foreach (var i in assignees)
        {
            var subject = await ProfileEntitiesService.GetSubjectEntity(i.ObjectId);
            if (subject != null)
            {
                subjects.Add(i.ObjectId.ToString(), subject);
            }
        }
        
        var assigneeGroups = await ObjectsAssigneeService.GetClientAssigningObjects(client, ObjectTypes.Group);
        foreach (var group in assigneeGroups)
        {
            var groupSubjects = await DbContext.ProfileSubjects
                .Where(x => x.GroupId == group.ObjectId)
                .Select(x => x)
                .ToListAsync();
            foreach (var i in groupSubjects)
            {
                var cp = await ProfileEntitiesService.GetSubjectEntity(i.Id);
                if (cp == null)
                    continue;
                subjects.TryAdd(i.Id.ToString(), cp);
            }
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
            return NotFound(new Error("找不到请求的对象"));
        var assignees = await ObjectsAssigneeService.GetClientAssigningObjects(client, ObjectTypes.ProfileTimeLayout);
        var timeLayouts = new ObservableDictionary<string, TimeLayout>();
        foreach (var i in assignees)
        {
            var tl = await ProfileEntitiesService.GetTimeLayoutEntity(i.ObjectId);
            if (tl == null)
                continue;
            timeLayouts.TryAdd(i.ObjectId.ToString(), tl);
        }

        var assigneeGroups = await ObjectsAssigneeService.GetClientAssigningObjects(client, ObjectTypes.Group);
        foreach (var group in assigneeGroups)
        {
            var groupTimeLayouts = await DbContext.ProfileTimelayouts
                .Where(x => x.GroupId == group.ObjectId)
                .Select(x => x)
                .ToListAsync();
            foreach (var i in groupTimeLayouts)
            {
                var tl = await ProfileEntitiesService.GetTimeLayoutEntity(i.Id);
                if (tl == null)
                    continue;
                timeLayouts.TryAdd(i.Id.ToString(), tl);
            }
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
            return NotFound(new Error("找不到请求的对象"));
        var assignees = await ObjectsAssigneeService.GetClientAssigningObjects(client, ObjectTypes.ProfileClassPlan);
        var classPlans = new ObservableDictionary<string, ClassPlan>();
        foreach (var i in assignees)
        {
            var cp = await ProfileEntitiesService.GetClassPlanEntity(i.ObjectId);
            if (cp != null)
            {
                classPlans.Add(i.ObjectId.ToString(), cp);
            }
        }
        
        var assigneeGroups = await ObjectsAssigneeService.GetClientAssigningObjects(client, ObjectTypes.Group);
        foreach (var group in assigneeGroups)
        {
            var groupClassPlans = await DbContext.ProfileClassplans
                .Where(x => x.GroupId == group.ObjectId)
                .Select(x => x)
                .ToListAsync();
            foreach (var i in groupClassPlans)
            {
                var cp = await ProfileEntitiesService.GetClassPlanEntity(i.Id);
                if (cp == null)
                    continue;
                classPlans.TryAdd(i.Id.ToString(), cp);
            }
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
            return NotFound(new Error("找不到请求的对象"));
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
            return NotFound(new Error("找不到请求的对象"));
        var assignees = await ObjectsAssigneeService.GetClientAssigningObjectsLeveled(client, ObjectTypes.AppSettings);
        foreach (var setting in assignees.Select(i => DbContext.Settings.FirstOrDefault(x => x.Id == i.ObjectId)).OfType<Setting>())
        {
            return Ok(setting.Settings ?? "{}");
        }
        return Ok(new ManagementPolicy());
    }
}