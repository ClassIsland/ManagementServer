namespace ClassIsland.ManagementServer.Server.Models.Identity;

public class UserCreateBody
{
    public User User { get; set; } = new();
    
    public string Password { get; set; } = "";
}