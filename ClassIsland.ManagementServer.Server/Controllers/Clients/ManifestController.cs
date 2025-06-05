using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Enums;
using ClassIsland.ManagementServer.Server.Models;
using ClassIsland.ManagementServer.Server.Services;
using ClassIsland.Shared.Enums;
using ClassIsland.Shared.Models.Management;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Controllers.Clients;

[ApiController]
[Route("api/v1/client/{cuid}/manifest")]
public class ManifestController(ManagementServerContext dataContext, 
    ObjectsUpdateNotifyService objectsUpdateNotifyService, IConfiguration configuration) : ControllerBase
{
    private ManagementServerContext DataContext { get; } = dataContext;

    private ObjectsUpdateNotifyService ObjectsUpdateNotifyService { get; } = objectsUpdateNotifyService;
    public IConfiguration Configuration { get; } = configuration;

    [HttpGet]
    public async Task<IActionResult> GetManifest([FromRoute] Guid cuid)
    {
        var client = DataContext.Clients.FirstOrDefault(i => i.Cuid == cuid);
        if (client == null)
        {
            return NotFound(new Error("找不到请求的对象"));
        }

        var manifest = new ManagementManifest();
        await using var tran = await DataContext.Database.BeginTransactionAsync();
        
        var classplanUpdates = DataContext.ObjectUpdates.Where(
            x => x.TargetCuid == cuid && (x.ObjectType == ObjectTypes.ProfileClassPlan)).Select(x =>x);
        if (classplanUpdates.Any())
        {
            client.ClassPlanVersion++;
            await classplanUpdates.ExecuteDeleteAsync();
        }

        var timeLayoutUpdates = DataContext.ObjectUpdates.Where(
            x => x.TargetCuid == cuid && (x.ObjectType == ObjectTypes.ProfileTimeLayout)).Select(x => x);
        if (timeLayoutUpdates.Any())
        {
            client.TimeLayoutVersion++;
            await timeLayoutUpdates.ExecuteDeleteAsync();
        }

        var subjectUpdates = DataContext.ObjectUpdates.Where(
            x => x.TargetCuid == cuid && (x.ObjectType == ObjectTypes.ProfileSubject)).Select(x => x);
        if (subjectUpdates.Any())
        {
            client.SubjectsVersion++;
            await subjectUpdates.ExecuteDeleteAsync();
        }

        var policyUpdates = DataContext.ObjectUpdates.Where(
            x => x.TargetCuid == cuid && (x.ObjectType == ObjectTypes.Policy)).Select(x => x);
        if (policyUpdates.Any())
        {
            client.PolicyVersion++;
            await policyUpdates.ExecuteDeleteAsync();
        }

        var settingsUpdates = DataContext.ObjectUpdates.Where(
            x => x.TargetCuid == cuid && (x.ObjectType == ObjectTypes.AppSettings)).Select(x => x);
        if (settingsUpdates.Any())
        {
            client.DefaultSettingsVersion++;
            await settingsUpdates.ExecuteDeleteAsync();
        }

        var globalUpdates = DataContext.ObjectUpdates.Where(
            x => x.TargetCuid == cuid).Select(x => x);
        if (globalUpdates.Any())
        {
            client.ClassPlanVersion++;
            client.TimeLayoutVersion++;
            client.SubjectsVersion++;
            client.PolicyVersion++;
            client.DefaultSettingsVersion++;
            await globalUpdates.ExecuteDeleteAsync();
        }

        await tran.CommitAsync();
        await DataContext.SaveChangesAsync();
        return Ok(new ManagementManifest()
        {
            OrganizationName = (await DataContext.OrganizationSettings.FindAsync("OrganizationName"))?.Value ?? "",
            ServerKind = ManagementServerKind.ManagementServer,
            SubjectsSource = new ReVersionString {
                Value = "{host}/api/v1/client/{cuid}/subjects",
                Version = client.SubjectsVersion
            },
            TimeLayoutSource = new ReVersionString
            {
                Value = "{host}/api/v1/client/{cuid}/timelayouts",
                Version = client.TimeLayoutVersion
            },
            ClassPlanSource = new ReVersionString
            {
                Value = "{host}/api/v1/client/{cuid}/classplans",
                Version = client.ClassPlanVersion
            },
            PolicySource = new ReVersionString
            {
                Value = "{host}/api/v1/client/{cuid}/policy",
                Version = client.PolicyVersion
            },
            DefaultSettingsSource = new ReVersionString
            {
                Value = "{host}/api/v1/client/{cuid}/default-settings",
                Version = client.DefaultSettingsVersion
            }
        });
    }
    
}