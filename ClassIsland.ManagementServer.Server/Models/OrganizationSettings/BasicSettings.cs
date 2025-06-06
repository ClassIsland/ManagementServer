namespace ClassIsland.ManagementServer.Server.Models.OrganizationSettings;

public class BasicSettings
{
    public bool AllowUnregisteredClients { get; init; } = true;

    public string CustomPublicRootUrl { get; init; } = "";
    
    public string CustomPublicApiUrl { get; init; } = "";
    
    public string CustomPublicGrpcUrl { get; init; } = "";
    
    public bool AllowPublicRegister { get; init; } = false;

}