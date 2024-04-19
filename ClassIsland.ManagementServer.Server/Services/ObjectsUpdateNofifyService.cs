using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.ManagementServer.Server.Enums;

namespace ClassIsland.ManagementServer.Server.Services;

public class ObjectsUpdateNofifyService(ManagementServerContext context, ObjectsAssigneeService objectsAssigneeService)
{
    private ManagementServerContext DbContext { get; } = context;

    private ObjectsAssigneeService ObjectsAssigneeService { get; } = objectsAssigneeService;

    public void NotifyObjectUpdating(string id, ObjectTypes objectType)
    {
        var clients = ObjectsAssigneeService.GetObjectAssignedClients(id);
        foreach (var i in clients)
        {
            DbContext.ObjectUpdates.Add(new ObjectUpdate()
            {
                ObjectId = id,
                ObjectType = (int)objectType,
                TargetCuid = i.Cuid,
                UpdateTime = DateTime.Now
            });
        }
    }

    public void NotifyClientUpdated(string cuid, string id)
    {
        
    }

    public bool IsObjectUpdated(string id)
    {
        return false;
    }
}