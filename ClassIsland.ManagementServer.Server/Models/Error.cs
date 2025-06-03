namespace ClassIsland.ManagementServer.Server.Models;

public class Error(string message)
{
    public string Message { get; } = message;
}