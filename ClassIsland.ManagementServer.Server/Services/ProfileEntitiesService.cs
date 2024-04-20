using System.Text.Json;
using ClassIsland.Core.Models.Profile;
using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.ManagementServer.Server.Enums;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Services;

public class ProfileEntitiesService(ManagementServerContext context, 
    ObjectsUpdateNotifyService objectsUpdateNotifyService,
    ILogger<ProfileEntitiesService> logger)
{
    private ManagementServerContext DbContext { get; } = context;

    private ObjectsUpdateNotifyService ObjectsUpdateNotifyService { get; } = objectsUpdateNotifyService;

    private ILogger<ProfileEntitiesService> Logger { get; } = logger;

    public async void SetSubjectEntity(string id, Subject subject, bool replace)
    {
        Logger.LogDebug("处理科目：{}（{}）", id, subject.Name);
        var o = new ProfileSubject()
        {
            Id = id,
            Name = subject.Name,
            Initials = subject.Initial,
            AttachedObjects = JsonSerializer.Serialize(subject.AttachedObjects),
            // TODO: IsOutdoor = ...
        };
        if (DbContext.ProfileSubjects.Any(x => x.Id == id))
        {
            if (!replace)
                return;
            DbContext.Entry(o).State = EntityState.Modified;
        }
        else
        {
            DbContext.ProfileSubjects.Add(o);
        }
        await ObjectsUpdateNotifyService.NotifyObjectUpdatingAsync(id, ObjectTypes.ProfileSubject);
    }
    
    public async void SetTimeLayoutEntity(string id, TimeLayout timeLayout, bool replace)
    {
        Logger.LogDebug("处理时间表：{}（{}）", id, timeLayout.Name);
        var o = new ProfileTimelayout()
        {
            Id = id,
            Name = timeLayout.Name,
            AttachedObjects = JsonSerializer.Serialize(timeLayout.AttachedObjects)
        };
        if (DbContext.ProfileTimelayouts.Any(x => x.Id == id))
        {
            if (!replace)
                return;
            DbContext.Entry(o).State = EntityState.Modified;
            DbContext.ProfileTimelayoutTimepoints.Where(x => x.ParentId == id).ExecuteDelete();
        }
        else
        {
            DbContext.ProfileTimelayouts.Add(o);
        }

        var index = 0;
        foreach (var p in timeLayout.Layouts)
        {
            DbContext.ProfileTimelayoutTimepoints.Add(new ProfileTimelayoutTimepoint()
            {
                Parent = o,
                AttachedObjects = JsonSerializer.Serialize(p.AttachedObjects),
                Start = new TimeOnly(p.StartSecond.Hour, p.StartSecond.Minute, p.StartSecond.Second),
                End = new TimeOnly(p.EndSecond.Hour, p.EndSecond.Minute, p.EndSecond.Second),
                TimeType = p.TimeType,
                DefaultSubjectId = p.DefaultClassId,
                Index = index ++
            });
        }
        await ObjectsUpdateNotifyService.NotifyObjectUpdatingAsync(id, ObjectTypes.ProfileTimeLayout);
    }
    
    public async void SetClassPlanEntity(string id, ClassPlan classPlan, bool replace)
    {
        Logger.LogDebug("处理课表：{}（{}）", id, classPlan.Name);
        var o = new ProfileClassplan()
        {
            Id = id,
            Name = classPlan.Name,
            AttachedObjects = JsonSerializer.Serialize(classPlan.AttachedObjects),
            WeekDay = classPlan.TimeRule.WeekDay,
            WeekDiv = classPlan.TimeRule.WeekCountDiv,
            // TODO: IsEnabled = ...
        };
        if (DbContext.ProfileClassplans.Any(x => x.Id == id))
        {
            if (!replace)
                return;
            DbContext.Entry(o).State = EntityState.Modified;
            DbContext.ProfileClassplanClasses.Where(x => x.ParentId == id).ExecuteDelete();
        }
        else
        {
            DbContext.ProfileClassplans.Add(o);
        }
        var index = 0;
        foreach (var p in classPlan.Classes)
        {
            Logger.LogDebug("处理课程：{}", p.SubjectId);
            if (!DbContext.ProfileSubjects.Any(x => x.Id == p.SubjectId))
                continue;
            DbContext.ProfileClassplanClasses.Add(new ProfileClassplanClass()
            {
                Parent = o,
                // AttachedObjects = JsonSerializer.Serialize(p.AttachedObjects),  // TODO: 等到ClassIsland完成课程层面的附加信息开发后取消注释这个
                SubjectId = p.SubjectId,
                Index = index ++
            });
        }
        await ObjectsUpdateNotifyService.NotifyObjectUpdatingAsync(id, ObjectTypes.ProfileClassPlan);
    }

}