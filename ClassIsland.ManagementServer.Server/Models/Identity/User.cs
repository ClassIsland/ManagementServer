using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ClassIsland.ManagementServer.Server.Models.Identity;

public class User : IdentityUser
{
    [PersonalData]
    [MaxLength(32)]
    public string Name { get; set; } = "";
    
    public DateTime CreatedTime { get; set; } = DateTime.Now;
    
}