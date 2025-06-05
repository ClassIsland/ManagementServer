using System.ComponentModel.DataAnnotations;

namespace ClassIsland.ManagementServer.Server.Entities;

public class OrganizationSettings
{
    [Key]
    [StringLength(32)]
    public string Key { get; set; } = string.Empty;
    
    public string? Value { get; set; } = string.Empty;

    [StringLength(32)] public string Category { get; set; } = string.Empty;
}