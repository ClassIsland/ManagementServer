using System.Collections.ObjectModel;
using System.Text.Json;
using ClassIsland.ManagementServer.Server.ComponentModels;
using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.ManagementServer.Server.Enums;
using ClassIsland.ManagementServer.Server.Helpers;
using ClassIsland.ManagementServer.Server.Models;
using ClassIsland.Shared.Models.Profile;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Services;

public class ProfileEntitiesService(
    ManagementServerContext context,
    ObjectsUpdateNotifyService objectsUpdateNotifyService,
    ILogger<ProfileEntitiesService> logger,
    IConfiguration configuration,
    ObjectsCacheService objectsCacheService)
{
    private ManagementServerContext DbContext { get; } = context;

    private ObjectsUpdateNotifyService ObjectsUpdateNotifyService { get; } = objectsUpdateNotifyService;

    private ILogger<ProfileEntitiesService> Logger { get; } = logger;
    public IConfiguration Configuration { get; } = configuration;
    public ObjectsCacheService ObjectsCacheService { get; } = objectsCacheService;

    public async Task SetSubjectEntity(Guid id, Subject subject, bool replace, Guid? groupId = null)
    {
        Logger.LogDebug("处理科目：{}（{}）", id, subject.Name);
        var o = new ProfileSubject()
        {
            Id = id,
            Name = subject.Name,
            Initials = subject.Initial,
            AttachedObjects = subject.AttachedObjects,
            IsOutDoor = subject.IsOutDoor,
            GroupId = groupId ?? ProfileGroup.DefaultGroupId
        };
        var raw = await DbContext.ProfileSubjects.FirstOrDefaultAsync(x => x.Id == id);
        if (raw != null)
        {
            if (!replace)
                return;
            if (groupId == null)
            {
                o.GroupId = raw.GroupId; // 保持原有分组
            }

            DbContext.Entry(raw).State = EntityState.Detached;
            DbContext.Entry(o).State = EntityState.Modified;
            o.CreatedTime = raw.CreatedTime;
        }
        else
        {
            await DbContext.ProfileSubjects.AddAsync(o);
        }

        ObjectsCacheService.InvalidateCache(ObjectTypes.ProfileSubject, id);
        await ObjectsUpdateNotifyService.NotifyObjectUpdatingAsync(id, ObjectTypes.ProfileSubject);
    }

    public async Task SetTimeLayoutEntity(Guid id, TimeLayout timeLayout, bool replace, Guid? groupId = null)
    {
        Logger.LogDebug("处理时间表：{}（{}）", id, timeLayout.Name);
        var o = new ProfileTimeLayout()
        {
            Id = id,
            Name = timeLayout.Name,
            AttachedObjects = timeLayout.AttachedObjects,
            GroupId = groupId ?? ProfileGroup.DefaultGroupId
        };
        var raw = await DbContext.ProfileTimelayouts.FirstOrDefaultAsync(x => x.Id == id);
        if (raw != null)
        {
            if (!replace)
                return;
            if (groupId == null)
            {
                o.GroupId = raw.GroupId; // 保持原有分组
            }
            DbContext.Entry(raw).State = EntityState.Detached;
            DbContext.Entry(o).State = EntityState.Modified;
            await DbContext.ProfileTimelayoutTimepoints.Where(x => x.ParentId == id).ExecuteDeleteAsync();
            o.CreatedTime = raw.CreatedTime;
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
                Index = index++,
                CreatedTime = raw?.CreatedTime ?? DateTime.Now,
            });
        }

        ObjectsCacheService.InvalidateCache(ObjectTypes.ProfileTimeLayout, id);
        await ObjectsUpdateNotifyService.NotifyObjectUpdatingAsync(id, ObjectTypes.ProfileTimeLayout);
    }

    public async Task SetClassPlanEntity(Guid id, ClassPlan classPlan, bool replace, Guid? groupId = null)
    {
        Logger.LogDebug("处理课表：{}（{}）", id, classPlan.Name);
        var timeLayout = await DbContext.ProfileTimelayouts.FirstOrDefaultAsync(x =>
            x.Id == GuidHelpers.TryParseGuidOrEmpty(classPlan.TimeLayoutId));
        if (timeLayout == null)
        {
            Logger.LogWarning("课表时间表不存在：{}", classPlan.TimeLayoutId);
            return;
        }
        var o = new ProfileClassplan()
        {
            Id = id,
            Name = classPlan.Name,
            AttachedObjects = classPlan.AttachedObjects,
            WeekDay = classPlan.TimeRule.WeekDay,
            WeekDiv = classPlan.TimeRule.WeekCountDiv,
            WeekCountDivTotal = classPlan.TimeRule.WeekCountDivTotal,
            TimeLayout = timeLayout,
            IsEnabled = classPlan.IsEnabled,
            GroupId = classPlan.GroupId ?? groupId ?? ProfileGroup.DefaultGroupId
        };
        var cp = await DbContext.ProfileClassplans.FirstOrDefaultAsync(x => x.Id == id);
        if (cp != null)
        {
            if (!replace)
                return;
            if (groupId == null && classPlan.GroupId == null)
            {
                o.GroupId = cp.GroupId; // 保持原有分组
            }
            DbContext.Entry(cp).State = EntityState.Detached;
            DbContext.Entry(o).State = EntityState.Modified;
            await DbContext.ProfileClassplanClasses.Where(x => x.ParentId == id).ExecuteDeleteAsync();
            o.CreatedTime = cp.CreatedTime;
        }
        else
        {
            await DbContext.ProfileClassplans.AddAsync(o);
        }

        var index = 0;
        foreach (var p in classPlan.Classes)
        {
            Logger.LogDebug("处理课程：{}", p.SubjectId);
            var subject =
                await DbContext.ProfileSubjects.FirstOrDefaultAsync(x =>
                    x.Id == GuidHelpers.TryParseGuidOrEmpty(p.SubjectId));
            if (subject == null)
                continue;
            await DbContext.ProfileClassplanClasses.AddAsync(new ProfileClassPlanClass()
            {
                Parent = o,
                // AttachedObjects = JsonSerializer.Serialize(p.AttachedObjects),  // TODO: 等到ClassIsland完成课程层面的附加信息开发后取消注释这个
                Subject = subject,
                Index = index++,
                CreatedTime = cp?.CreatedTime ?? DateTime.Now
            });
        }

        ObjectsCacheService.InvalidateCache(ObjectTypes.ProfileClassPlan, id);
        await ObjectsUpdateNotifyService.NotifyObjectUpdatingAsync(id, ObjectTypes.ProfileClassPlan);
    }
    
    private DateTime ConvertTimeOnly(TimeOnly t)
    {
        var now = DateTime.Now;
        return new DateTime(now.Year, now.Month, now.Day, t.Hour, t.Minute, t.Second);
    }

    public async Task<ClassPlan?> GetClassPlanEntity(Guid id)
    {
        Logger.LogDebug("获取课表：{}", id);
        if (ObjectsCacheService.ClassPlanCache.TryGet(id, out var cached))
        {
            Logger.LogTrace("命中对象缓存（课表） {}", id);
            return cached;
        }
        var cp = await DbContext.ProfileClassplans.FirstOrDefaultAsync(x => x.Id == id);
        if (cp == null)
            return null;
        var c = new ObservableCollection<ClassInfo>((
                await DbContext.ProfileClassplanClasses
                .Where(x => x.ParentId == cp.Id)
                .OrderBy(x => x.Index)
                .Select(x => x)
                .ToListAsync())
            .Select(x =>
                new ClassInfo()
                {
                    SubjectId = x.SubjectId.ToString(),
                    // TODO: AttachedObjects =
                    //     JsonSerializer.Deserialize<Dictionary<string, object?>>(x.AttachedObjects ?? "{}",
                    //         JsonOptions) ?? new()
                }
            )
        );
        var classPlan = new ClassPlan()
        {
            Name = cp.Name,
            TimeRule = new TimeRule()
            {
                WeekDay = cp.WeekDay,
                WeekCountDiv = cp.WeekDiv,
                WeekCountDivTotal = cp.WeekCountDivTotal
            },
            TimeLayoutId = cp.TimeLayoutId.ToString(),
            Classes = c,
            AttachedObjects = cp.AttachedObjects,
            IsEnabled = cp.IsEnabled,
            GroupId = cp.GroupId
        };
        ObjectsCacheService.ClassPlanCache.Add(id, classPlan);
        return classPlan;
    }

    public async Task<TimeLayout?> GetTimeLayoutEntity(Guid id)
    {
        Logger.LogDebug("获取时间表：{}", id);
        if (ObjectsCacheService.TimeLayoutCache.TryGet(id, out var cached))
        {
            Logger.LogTrace("命中对象缓存（时间表） {}", id);
            return cached;
        }
        var tl = await DbContext.ProfileTimelayouts.FirstOrDefaultAsync(x => x.Id == id);
        if (tl == null)
            return null;
        var tp = new ObservableCollection<TimeLayoutItem>((
                await DbContext.ProfileTimelayoutTimepoints
                .Where(x => x.ParentId == tl.Id)
                .OrderBy(x => x.Index)
                .Select(x => x)
                .ToListAsync())
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
        var timeLayout = new TimeLayout()
        {
            Name = tl.Name,
            Layouts = tp,
            AttachedObjects = tl.AttachedObjects
        };
        ObjectsCacheService.TimeLayoutCache.Add(id, timeLayout);
        return timeLayout;
    }
    
    public async Task<Subject?> GetSubjectEntity(Guid id)
    {
        Logger.LogDebug("获取科目：{}", id);
        if (ObjectsCacheService.SubjectCache.TryGet(id, out var cached))
        {
            Logger.LogTrace("命中对象缓存（科目） {}", id);
            return cached;
        }
        var s = await DbContext.ProfileSubjects.FirstOrDefaultAsync(x => x.Id == id);
        if (s == null)
            return null;
        var subject = new Subject()
        {
            Name = s.Name,
            Initial = s.Initials,
            IsOutDoor = s.IsOutDoor,
            TeacherName = "",
            AttachedObjects = s.AttachedObjects
        };
        ObjectsCacheService.SubjectCache.Add(id, subject);
        return subject;
    }
}