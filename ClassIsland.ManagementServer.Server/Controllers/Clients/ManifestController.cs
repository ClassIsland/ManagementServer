using ClassIsland.Core.Enums;
using ClassIsland.Core.Models.Management;
using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Enums;
using ClassIsland.ManagementServer.Server.Services;
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
    public IActionResult GetManifest([FromRoute] string cuid)
    {
        var client = DataContext.Clients.FirstOrDefault(i => i.Cuid == cuid);
        if (client == null)
        {
            return NotFound();
        }

        var manifest = new ManagementManifest();
        using var tran = DataContext.Database.BeginTransaction();

        client.ClassplanVersion ??= 0;
        client.TimeLayoutVersion ??= 0;
        client.SubjectsVersion ??= 0;
        client.PolicyVersion ??= 0;
        client.SubjectsVersion ??= 0;
        var classplanUpdates = DataContext.ObjectUpdates.Where(
            x => x.TargetCuid == cuid && (x.ObjectType == (int)ObjectTypes.ProfileClassPlan)).Select(x =>x);
        if (classplanUpdates.Any())
        {
            client.ClassplanVersion++;
            classplanUpdates.ExecuteDelete();
        }

        var timeLayoutUpdates = DataContext.ObjectUpdates.Where(
            x => x.TargetCuid == cuid && (x.ObjectType == (int)ObjectTypes.ProfileTimeLayout)).Select(x => x);
        if (timeLayoutUpdates.Any())
        {
            client.TimeLayoutVersion++;
            timeLayoutUpdates.ExecuteDelete();
        }

        var subjectUpdates = DataContext.ObjectUpdates.Where(
            x => x.TargetCuid == cuid && (x.ObjectType == (int)ObjectTypes.ProfileSubject)).Select(x => x);
        if (subjectUpdates.Any())
        {
            client.SubjectsVersion++;
            subjectUpdates.ExecuteDelete();
        }

        var policyUpdates = DataContext.ObjectUpdates.Where(
            x => x.TargetCuid == cuid && (x.ObjectType == (int)ObjectTypes.Policy)).Select(x => x);
        if (policyUpdates.Any())
        {
            client.PolicyVersion++;
            policyUpdates.ExecuteDelete();
        }

        var settingsUpdates = DataContext.ObjectUpdates.Where(
            x => x.TargetCuid == cuid && (x.ObjectType == (int)ObjectTypes.AppSettings)).Select(x => x);
        if (settingsUpdates.Any())
        {
            client.DefaultSettingsVersion++;
            settingsUpdates.ExecuteDelete();
        }

        var globalUpdates = DataContext.ObjectUpdates.Where(
            x => x.TargetCuid == cuid && (x.ObjectType == null)).Select(x => x);
        if (globalUpdates.Any())
        {
            client.ClassplanVersion++;
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
                Version = client.SubjectsVersion!.Value
            },
            TimeLayoutSource = new ReVersionString
            {
                Value = "{host}/api/v1/client/{cuid}/timelayouts",
                Version = client.TimeLayoutVersion!.Value
            },
            ClassPlanSource = new ReVersionString
            {
                Value = "{host}/api/v1/client/{cuid}/classplans",
                Version = client.ClassplanVersion!.Value
            },
            PolicySource = new ReVersionString
            {
                Value = "{host}/api/v1/client/{cuid}/policy",
                Version = client.PolicyVersion!.Value
            },
            DefaultSettingsSource = new ReVersionString
            {
                Value = "{host}/api/v1/client/{cuid}/default-settings",
                Version = client.DefaultSettingsVersion!.Value
            }
        });
    }
    
}