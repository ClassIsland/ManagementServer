using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Enums;
using ClassIsland.ManagementServer.Server.Services;
using ClassIsland.Shared.Enums;
using ClassIsland.Shared.Models.Management;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Controllers.Clients;

[ApiController]
[Route("api/v1/client/{cuid}/manifest")]
public class ManifestController(ManagementServerContext dataContext, 
    ObjectsUpdateNotifyService objectsUpdateNotifyService) : ControllerBase
{
    private ManagementServerContext DataContext { get; } = dataContext;

    private ObjectsUpdateNotifyService ObjectsUpdateNotifyService { get; } = objectsUpdateNotifyService;

    [HttpGet]
    public IActionResult GetManifest([FromRoute] Guid cuid)
    {
        var client = DataContext.Clients.FirstOrDefault(i => i.Cuid == cuid);
        if (client == null)
        {
            return NotFound();
        }

        var manifest = new ManagementManifest();
        using var tran = DataContext.Database.BeginTransaction();
        
        var classplanUpdates = DataContext.ObjectUpdates.Where(
            x => x.TargetCuid == cuid && (x.ObjectType == ObjectTypes.ProfileClassPlan)).Select(x =>x);
        if (classplanUpdates.Any())
        {
            client.ClassPlanVersion++;
            classplanUpdates.ExecuteDelete();
        }

        var timeLayoutUpdates = DataContext.ObjectUpdates.Where(
            x => x.TargetCuid == cuid && (x.ObjectType == ObjectTypes.ProfileTimeLayout)).Select(x => x);
        if (timeLayoutUpdates.Any())
        {
            client.TimeLayoutVersion++;
            timeLayoutUpdates.ExecuteDelete();
        }

        var subjectUpdates = DataContext.ObjectUpdates.Where(
            x => x.TargetCuid == cuid && (x.ObjectType == ObjectTypes.ProfileSubject)).Select(x => x);
        if (subjectUpdates.Any())
        {
            client.SubjectsVersion++;
            subjectUpdates.ExecuteDelete();
        }

        var policyUpdates = DataContext.ObjectUpdates.Where(
            x => x.TargetCuid == cuid && (x.ObjectType == ObjectTypes.Policy)).Select(x => x);
        if (policyUpdates.Any())
        {
            client.PolicyVersion++;
            policyUpdates.ExecuteDelete();
        }

        var settingsUpdates = DataContext.ObjectUpdates.Where(
            x => x.TargetCuid == cuid && (x.ObjectType == ObjectTypes.AppSettings)).Select(x => x);
        if (settingsUpdates.Any())
        {
            client.DefaultSettingsVersion++;
            settingsUpdates.ExecuteDelete();
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
            globalUpdates.ExecuteDelete();
        }

        tran.Commit();
        DataContext.SaveChanges();
        return Ok(new ManagementManifest()
        {
            OrganizationName = "", // TODO: 读取组织名
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