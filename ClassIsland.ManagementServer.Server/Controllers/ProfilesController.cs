using System.Text.Json;
using ClassIsland.Core.Models.Profile;
using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.ManagementServer.Server.Enums;
using ClassIsland.ManagementServer.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Controllers;

[ApiController]
[Route("api/v1/profiles")]
public class ProfilesController(ManagementServerContext dbContext, ILogger<ProfilesController> logger) : ControllerBase
{
    private ManagementServerContext DbContext { get; } = dbContext;

    private ILogger<ProfilesController> Logger { get; } = logger;

    [HttpPost("upload")]
    public IActionResult UploadProfile([FromBody] Profile profile, [FromQuery] bool replace=false)
    {
        using var tran = DbContext.Database.BeginTransaction();
        // 处理科目
        foreach (var i in profile.Subjects)
        {
            Logger.LogDebug("处理科目：{}（{}）", i.Key, i.Value.Name);
            var o = new ProfileSubject()
            {
                Id = i.Key,
                Name = i.Value.Name,
                Initials = i.Value.Initial,
                AttachedObjects = JsonSerializer.Serialize(i.Value.AttachedObjects),
                // TODO: IsOutdoor = ...
            };
            if (DbContext.ProfileSubjects.Any(x => x.Id == i.Key))
            {
                if (!replace)
                    continue;
                DbContext.Entry(o).State = EntityState.Modified;
            }
            else
            {
                DbContext.ProfileSubjects.Add(o);
            }
        }
        
        // 处理时间表
        foreach (var i in profile.TimeLayouts)
        {
            Logger.LogDebug("处理时间表：{}（{}）", i.Key, i.Value.Name);
            var o = new ProfileTimelayout()
            {
                Id = i.Key,
                Name = i.Value.Name,
                AttachedObjects = JsonSerializer.Serialize(i.Value.AttachedObjects)
            };
            if (DbContext.ProfileTimelayouts.Any(x => x.Id == i.Key))
            {
                if (!replace)
                    continue;
                DbContext.Entry(o).State = EntityState.Modified;
                DbContext.ProfileTimelayoutTimepoints.Where(x => x.ParentId == i.Key).ExecuteDelete();
            }
            else
            {
                DbContext.ProfileTimelayouts.Add(o);
            }

            var index = 0;
            foreach (var p in i.Value.Layouts)
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
        }
        
        // 处理课表
        foreach (var i in profile.ClassPlans)
        {
            Logger.LogDebug("处理课表：{}（{}）", i.Key, i.Value.Name);
            var o = new ProfileClassplan()
            {
                Id = i.Key,
                Name = i.Value.Name,
                AttachedObjects = JsonSerializer.Serialize(i.Value.AttachedObjects),
                WeekDay = i.Value.TimeRule.WeekDay,
                WeekDiv = i.Value.TimeRule.WeekCountDiv,
                // TODO: IsEnabled = ...
            };
            if (DbContext.ProfileClassplans.Any(x => x.Id == i.Key))
            {
                if (!replace)
                    continue;
                DbContext.Entry(o).State = EntityState.Modified;
                DbContext.ProfileClassplanClasses.Where(x => x.ParentId == i.Key).ExecuteDelete();
            }
            else
            {
                DbContext.ProfileClassplans.Add(o);
            }
            var index = 0;
            foreach (var p in i.Value.Classes)
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
        }

        tran.Commit();
        DbContext.SaveChanges();
        return Ok();
    }
    
    
}