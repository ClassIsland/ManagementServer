using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace ClassIsland.ManagementServer.Server.Models.OrganizationSettings;

public class BrandInfo
{
    public string OrganizationName { get; init; } = string.Empty;

    public string? LogoUrl { get; init; } = null;

    public string? CustomLoginBanner { get; init; } = null;
    
    public string? LoginFormPlacement { get; init; } = "left";

}