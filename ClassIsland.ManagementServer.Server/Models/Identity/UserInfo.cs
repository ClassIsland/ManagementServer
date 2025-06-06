using ClassIsland.ManagementServer.Server.Abstractions.Entities;

namespace ClassIsland.ManagementServer.Server.Models.Identity;

public class UserInfo : IObjectWithTime
{
    public string UserName { get; init; } = "";
    
    public string Name { get; set; } = "";

    public string EmailAddress { get; set; } = "";

    public string PhoneNumber { get; set; } = "";

    public string Id { get; set; } = "";

    public DateTime CreatedTime { get; } = DateTime.Now;
    public DateTime UpdatedTime { get; } = DateTime.Now;
}