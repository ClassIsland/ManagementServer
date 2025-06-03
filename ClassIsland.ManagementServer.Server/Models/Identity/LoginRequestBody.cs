namespace ClassIsland.ManagementServer.Server.Models.Identity;

public class LoginRequestBody
{
    public string Username { get; set; } = "";
    
    public string Password { get; set; } = "";
}