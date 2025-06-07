using ClassIsland.ManagementServer.Server.Authorization;
using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Models;
using ClassIsland.ManagementServer.Server.Models.OrganizationSettings;
using ClassIsland.ManagementServer.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Controllers;

[Authorize(Roles = Roles.Admin)]
[ApiController]
[Route("api/v1/settings")]
public class OrganizationSettingsController(ManagementServerContext dbContext, OrganizationSettingsService organizationSettingsService) : ControllerBase
{
    public ManagementServerContext DbContext { get; } = dbContext;
    public OrganizationSettingsService OrganizationSettingsService { get; } = organizationSettingsService;

    [AllowAnonymous]
    [HttpGet("brand")]
    public async Task<IActionResult> GetBrandSettings()
    {
        var settings = await DbContext.OrganizationSettings
            .Where(s => s.Category == "Brand")
            .ToDictionaryAsync(s => s.Key, s => s.Value);

        return Ok(new BrandInfo()
        {
            OrganizationName = settings.GetValueOrDefault(nameof(BrandInfo.OrganizationName), string.Empty)!,
            LogoUrl = settings.GetValueOrDefault(nameof(BrandInfo.LogoUrl), null!),
            CustomLoginBanner = settings.GetValueOrDefault(nameof(BrandInfo.CustomLoginBanner), null!),
            LoginFormPlacement = settings.GetValueOrDefault(nameof(BrandInfo.LoginFormPlacement), "left")
        });
    }
    
    [HttpPost("brand")]
    public async Task<IActionResult> UpdateBrandSettings([FromBody] BrandInfo body)
    {
        await OrganizationSettingsService.SetOrganizationSettings(nameof(BrandInfo.OrganizationName), "Brand", body.OrganizationName);
        await OrganizationSettingsService.SetOrganizationSettings(nameof(BrandInfo.LogoUrl), "Brand", body.LogoUrl);
        await OrganizationSettingsService.SetOrganizationSettings(nameof(BrandInfo.CustomLoginBanner), "Brand", body.CustomLoginBanner);
        await OrganizationSettingsService.SetOrganizationSettings(nameof(BrandInfo.LoginFormPlacement), "Brand", body.LoginFormPlacement);
        await DbContext.SaveChangesAsync();
        
        return Ok();
    }

    [HttpGet("basic")]
    public async Task<IActionResult> GetBasicSettings()
    {
        var settings = await OrganizationSettingsService.GetSettingsByCategory("Basic");
        
        return Ok(new BasicSettings()
        {
            AllowUnregisteredClients = !bool.TryParse(settings.GetValueOrDefault(nameof(BasicSettings.AllowUnregisteredClients), "true"), out var r1) || r1,
            AllowPublicRegister = bool.TryParse(settings.GetValueOrDefault(nameof(BasicSettings.AllowPublicRegister), "false"), out var r2) && r2,
            CustomPublicApiUrl = settings.GetValueOrDefault(nameof(BasicSettings.CustomPublicApiUrl), "")!,
            CustomPublicGrpcUrl = settings.GetValueOrDefault(nameof(BasicSettings.CustomPublicGrpcUrl), "")!,
            CustomPublicRootUrl = settings.GetValueOrDefault(nameof(BasicSettings.CustomPublicRootUrl), "")!
        });
    }
    
    [HttpPost("basic")]
    public async Task<IActionResult> UpdateBasicSettings([FromBody] BasicSettings body)
    {
        await OrganizationSettingsService.SetOrganizationSettings(nameof(BasicSettings.AllowPublicRegister), "Basic", body.AllowPublicRegister.ToString());
        await OrganizationSettingsService.SetOrganizationSettings(nameof(BasicSettings.AllowUnregisteredClients), "Basic", body.AllowUnregisteredClients.ToString());
        await OrganizationSettingsService.SetOrganizationSettings(nameof(BasicSettings.CustomPublicApiUrl), "Basic", body.CustomPublicApiUrl);
        await OrganizationSettingsService.SetOrganizationSettings(nameof(BasicSettings.CustomPublicGrpcUrl), "Basic", body.CustomPublicGrpcUrl);
        await OrganizationSettingsService.SetOrganizationSettings(nameof(BasicSettings.CustomPublicRootUrl), "Basic", body.CustomPublicRootUrl);
        
        await DbContext.SaveChangesAsync();
        
        return Ok();
    }

    [HttpPost("complete-oobe")]
    public async Task<IActionResult> CompleteOobe()
    {
        await OrganizationSettingsService.SetOrganizationSettings("IsOobeCompleted", "Oobe", "true");
        await DbContext.SaveChangesAsync();
        return Ok();
    }
}