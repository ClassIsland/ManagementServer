using ClassIsland.ManagementServer.Server.Abstractions.Entities;

namespace ClassIsland.ManagementServer.Server.Models.Identity;

public class UserInfo : IObjectWithTime
{
    public string UserName { get; init; } = "";
    
    public string Name { get; set; } = "";

    public string EmailAddress { get; set; } = "";

    public string PhoneNumber { get; set; } = "";

    public string Id { get; set; } = "";

    public bool AllowChangePassword { get; set; } = true;

    public List<string> Roles { get; set; } = [];

    public bool RedirectToOobe { get; set; } = false;

    public DateTime CreatedTime { get; set; } = DateTime.Now;
    public DateTime UpdatedTime { get; set; } = DateTime.Now;
}