using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Models;
using ClassIsland.ManagementServer.Server.Models.OrganizationSettings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/settings")]
public class OrganizationSettingsController(ManagementServerContext dbContext) : ControllerBase
{
    public ManagementServerContext DbContext { get; } = dbContext;
    
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
        await DbContext.SetOrganizationSettings(nameof(BrandInfo.OrganizationName), "Brand", body.OrganizationName);
        await DbContext.SetOrganizationSettings(nameof(BrandInfo.LogoUrl), "Brand", body.LogoUrl);
        await DbContext.SetOrganizationSettings(nameof(BrandInfo.CustomLoginBanner), "Brand", body.CustomLoginBanner);
        await DbContext.SetOrganizationSettings(nameof(BrandInfo.LoginFormPlacement), "Brand", body.LoginFormPlacement);
        await DbContext.SaveChangesAsync();
        
        return Ok();
    } 
}