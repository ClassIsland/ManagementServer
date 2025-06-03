namespace ClassIsland.ManagementServer.Server.Models.Identity;

public class ChangePasswordRequestBody
{
    public string OldPassword { get; set; } = string.Empty;
    
    public string NewPassword { get; set; } = string.Empty;
}