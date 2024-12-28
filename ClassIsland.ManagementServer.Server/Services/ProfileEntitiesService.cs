using System.Text.Json;
using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.ManagementServer.Server.Enums;
using ClassIsland.ManagementServer.Server.Helpers;
using ClassIsland.Shared.Models.Profile;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Services;

public class ProfileEntitiesService(ManagementServerContext context, 
    ObjectsUpdateNotifyService objectsUpdateNotifyService,
    ILogger<ProfileEntitiesService> logger)
{
    private ManagementServerContext DbContext { get; } = context;

    private ObjectsUpdateNotifyService ObjectsUpdateNotifyService { get; } = objectsUpdateNotifyService;

    private ILogger<ProfileEntitiesService> Logger { get; } = logger;

    public async Task SetSubjectEntity(Guid id, Subject subject, bool replace)
    {
        Logger.LogDebug("处理科目：{}（{}）", id, subject.Name);
        var o = new ProfileSubject()
        {
            Id = id,
            Name = subject.Name,
            Initials = subject.Initial,
            AttachedObjects = subject.AttachedObjects,
            IsOutDoor = subject.IsOutDoor
        };
        if (await DbContext.ProfileSubjects.AnyAsync(x => x.Id == id))
        {
            if (!replace)
                return;
            DbContext.Entry(o).State = EntityState.Modified;
        }
        else
        {
            await DbContext.ProfileSubjects.AddAsync(o);
        }
        await ObjectsUpdateNotifyService.NotifyObjectUpdatingAsync(id, ObjectTypes.ProfileSubject);
    }
    
    public async Task SetTimeLayoutEntity(Guid id, TimeLayout timeLayout, bool replace)
    {
        Logger.LogDebug("处理时间表：{}（{}）", id, timeLayout.Name);
        var o = new ProfileTimeLayout()
        {
            Id = id,
            Name = timeLayout.Name,
            AttachedObjects = timeLayout.AttachedObjects
        };
        if (await DbContext.ProfileTimelayouts.AnyAsync(x => x.Id == id))
        {
            if (!replace)
                return;
            DbContext.Entry(o).State = EntityState.Modified;
            DbContext.ProfileTimelayoutTimepoints.Where(x => x.ParentId == id).ExecuteDelete();
        }
        else
        {
            await DbContext.ProfileTimelayouts.AddAsync(o);
        }

        var index = 0;
        foreach (var p in timeLayout.Layouts)
        {
            await DbContext.ProfileTimelayoutTimepoints.AddAsync(new ProfileTimeLayoutTimePoint()
            {
                Parent = o,
                AttachedObjects = p.AttachedObjects,
                Start = new TimeOnly(p.StartSecond.Hour, p.StartSecond.Minute, p.StartSecond.Second),
                End = new TimeOnly(p.EndSecond.Hour, p.EndSecond.Minute, p.EndSecond.Second),
                TimeType = p.TimeType,
                DefaultSubjectId = p.DefaultClassId,
                IsHideDefault = p.IsHideDefault,
                Index = index ++
            });
        }
        await ObjectsUpdateNotifyService.NotifyObjectUpdatingAsync(id, ObjectTypes.ProfileTimeLayout);
    }
    
    public async Task SetClassPlanEntity(Guid id, ClassPlan classPlan, bool replace)
    {
        Logger.LogDebug("处理课表：{}（{}）", id, classPlan.Name);
        var o = new ProfileClassplan()
        {
            Id = id,
            Name = classPlan.Name,
            AttachedObjects = classPlan.AttachedObjects,
            WeekDay = classPlan.TimeRule.WeekDay,
            WeekDiv = classPlan.TimeRule.WeekCountDiv,
            TimeLayout = await DbContext.ProfileTimelayouts.FirstOrDefaultAsync(x => x.Id == GuidHelpers.TryParseGuidOrEmpty(classPlan.TimeLayoutId)) ?? throw new Exception("TimeLayout not found"),
            IsEnabled = classPlan.IsEnabled
        };
        if (await DbContext.ProfileClassplans.AnyAsync(x => x.Id == id))
        {
            if (!replace)
                return;
            DbContext.Entry(o).State = EntityState.Modified;
            await DbContext.ProfileClassplanClasses.Where(x => x.ParentId == id).ExecuteDeleteAsync();
        }
        else
        {
            await DbContext.ProfileClassplans.AddAsync(o);
        }
        var index = 0;
        foreach (var p in classPlan.Classes)
        {
            Logger.LogDebug("处理课程：{}", p.SubjectId);
            var subject = await DbContext.ProfileSubjects.FirstOrDefaultAsync(x => x.Id == GuidHelpers.TryParseGuidOrEmpty(p.SubjectId));
            if (subject == null)
                continue;
            await DbContext.ProfileClassplanClasses.AddAsync(new ProfileClassPlanClass()
            {
                Parent = o,
                // AttachedObjects = JsonSerializer.Serialize(p.AttachedObjects),  // TODO: 等到ClassIsland完成课程层面的附加信息开发后取消注释这个
                Subject = subject,
                Index = index ++
            });
        }
        await ObjectsUpdateNotifyService.NotifyObjectUpdatingAsync(id, ObjectTypes.ProfileClassPlan);
    }

}