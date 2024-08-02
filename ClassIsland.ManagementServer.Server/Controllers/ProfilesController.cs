using System.Text.Json;
using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.ManagementServer.Server.Enums;
using ClassIsland.ManagementServer.Server.Models;
using ClassIsland.ManagementServer.Server.Services;
using ClassIsland.Shared.Models.Profile;
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
    public async Task<IActionResult> UploadProfile([FromBody] Profile profile, [FromQuery] bool replace=false)
    {
        await using var tran = await DbContext.Database.BeginTransactionAsync();
        // 处理科目
        foreach (var i in profile.Subjects)
        {
            await ProfileEntitiesService.SetSubjectEntity(i.Key, i.Value, replace);
        }
        
        // 处理时间表
        foreach (var i in profile.TimeLayouts)
        {
            await ProfileEntitiesService.SetTimeLayoutEntity(i.Key, i.Value, replace);
        }
        
        // 处理课表
        foreach (var i in profile.ClassPlans)
        {
            await ProfileEntitiesService.SetClassPlanEntity(i.Key, i.Value, replace);
        }

        await tran.CommitAsync();
        await DbContext.SaveChangesAsync();
        return Ok();
    }
    
    
}