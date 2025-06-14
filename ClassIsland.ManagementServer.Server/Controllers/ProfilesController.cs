using System.Text.Json;
using ClassIsland.ManagementServer.Server.Authorization;
using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.ManagementServer.Server.Enums;
using ClassIsland.ManagementServer.Server.Helpers;
using ClassIsland.ManagementServer.Server.Models;
using ClassIsland.ManagementServer.Server.Models.Identity;
using ClassIsland.ManagementServer.Server.Services;
using ClassIsland.Shared.Models.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/profiles")]
public class ProfilesController(ManagementServerContext dbContext, 
    ILogger<ProfilesController> logger, 
    ProfileEntitiesService profileEntitiesService) : ControllerBase
{
    private ManagementServerContext DbContext { get; } = dbContext;

    private ILogger<ProfilesController> Logger { get; } = logger;

    private ProfileEntitiesService ProfileEntitiesService { get; } = profileEntitiesService;

    [Authorize(Roles = Roles.ObjectsWrite)]
    [HttpPost("upload")]
    public async Task<IActionResult> UploadProfile([FromBody] Profile profile, [FromQuery] bool replace=false, [FromQuery] Guid? groupId=null)
    {
        groupId ??= ProfileGroup.DefaultGroupId;
        // 处理科目
        foreach (var i in profile.Subjects)
        {
            await ProfileEntitiesService.SetSubjectEntity(GuidHelpers.TryParseGuidOrEmpty(i.Key), i.Value, replace, groupId);
        }
        await DbContext.SaveChangesAsync();
        
        // 处理时间表
        foreach (var i in profile.TimeLayouts)
        {
            await ProfileEntitiesService.SetTimeLayoutEntity(GuidHelpers.TryParseGuidOrEmpty(i.Key), i.Value, replace, groupId);
        }
        await DbContext.SaveChangesAsync();
        
        // 处理课表
        foreach (var i in profile.ClassPlans)
        {
            await ProfileEntitiesService.SetClassPlanEntity(GuidHelpers.TryParseGuidOrEmpty(i.Key), i.Value, replace, groupId);
        }
        await DbContext.SaveChangesAsync();
        return Ok();
    }
    
    
}