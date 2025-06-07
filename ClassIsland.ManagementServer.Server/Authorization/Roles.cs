namespace ClassIsland.ManagementServer.Server.Authorization;

public static class Roles
{
    public const string Admin = "Admin";

    public const string ObjectsWrite = "ObjectsWrite";
        
    public const string ObjectsDelete = "ObjectsDelete";
    
    public const string ClientsWrite = "ClientsWrite";
    
    public const string ClientsDelete = "ClientsDelete";
    
    public const string CommandsUser = "CommandsUser";
    
    public const string UsersManager = "UsersManager";

    public static readonly List<string> AllRoles = [Admin, ObjectsWrite, ObjectsDelete, ClientsWrite, ClientsDelete, CommandsUser, UsersManager];
}