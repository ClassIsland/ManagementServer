using ClassIsland.ManagementServer.Server.Context;

namespace ClassIsland.ManagementServer.Server.Services;

public class ClassIslandObjectsService(ManagementServerContext context)
{
    private ManagementServerContext DbContext { get; } = context;

    public void NotifyObjectUpdating(string id)
    {
        
    }

    public void NotifyClientUpdated(string cuid, string id)
    {
        
    }
}