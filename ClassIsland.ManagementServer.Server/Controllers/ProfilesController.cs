using System.Text.Json;
using ClassIsland.Core.Models.Profile;
using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.ManagementServer.Server.Enums;
using ClassIsland.ManagementServer.Server.Models;
using ClassIsland.ManagementServer.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Controllers;

[ApiController]
[Route("api/v1/profiles")]
public class ProfilesController(ManagementServerContext dbContext, 
    ILogger<ProfilesController> logger, 
    ProfileEntitiesService profileEntitiesService) : ControllerBase
{
    private ManagementServerContext DbContext { get; } = dbContext;

    private ILogger<ProfilesController> Logger { get; } = logger;

    private ProfileEntitiesService ProfileEntitiesService { get; } = profileEntitiesService;

    [HttpPost("upload")]
    public IActionResult UploadProfile([FromBody] Profile profile, [FromQuery] bool replace=false)
    {
        using var tran = DbContext.Database.BeginTransaction();
        // 处理科目
        foreach (var i in profile.Subjects)
        {
            ProfileEntitiesService.SetSubjectEntity(i.Key, i.Value, replace);
        }
        
        // 处理时间表
        foreach (var i in profile.TimeLayouts)
        {
            ProfileEntitiesService.SetTimeLayoutEntity(i.Key, i.Value, replace);
        }
        
        // 处理课表
        foreach (var i in profile.ClassPlans)
        {
            ProfileEntitiesService.SetClassPlanEntity(i.Key, i.Value, replace);
        }

        tran.Commit();
        DbContext.SaveChanges();
        return Ok();
    }
    
    
}