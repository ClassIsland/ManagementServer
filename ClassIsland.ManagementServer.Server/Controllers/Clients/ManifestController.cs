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

        if (DataContext.ObjectUpdates.Any(
                x => x.TargetCuid == cuid && x.ObjectType == (int)ObjectTypes.ProfileClassPlan))
        {
            client.ClassplanVersion++;
        }
        if (DataContext.ObjectUpdates.Any(
                x => x.TargetCuid == cuid && x.ObjectType == (int)ObjectTypes.ProfileTimeLayout))
        {
            client.TimeLayoutVersion++;
        }
        if (DataContext.ObjectUpdates.Any(
                x => x.TargetCuid == cuid && x.ObjectType == (int)ObjectTypes.ProfileSubject))
        {
            client.SubjectsVersion++;
        }
        if (DataContext.ObjectUpdates.Any(
                x => x.TargetCuid == cuid && x.ObjectType == (int)ObjectTypes.Policy))
        {
            client.PolicyVersion++;
        }
        if (DataContext.ObjectUpdates.Any(
                x => x.TargetCuid == cuid && x.ObjectType == (int)ObjectTypes.AppSettings))
        {
            client.DefaultSettingsVersion++;
        }

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