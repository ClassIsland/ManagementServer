using System.ComponentModel.DataAnnotations;
using ClassIsland.ManagementServer.Server.Abstractions.Entities;
using Microsoft.AspNetCore.Identity;

namespace ClassIsland.ManagementServer.Server.Models.Identity;

public class User : IdentityUser, IObjectWithTime
{
    [PersonalData]
    [MaxLength(32)]
    public string Name { get; set; }
    
    public DateTime CreatedTime { get; set; } = DateTime.Now;
    public DateTime UpdatedTime { get; set; } = DateTime.Now;
}