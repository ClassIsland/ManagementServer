using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.ManagementServer.Server.Enums;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Services;

public class ObjectsUpdateNotifyService(ManagementServerContext context, ObjectsAssigneeService objectsAssigneeService)
{
    private ManagementServerContext DbContext { get; } = context;

    private ObjectsAssigneeService ObjectsAssigneeService { get; } = objectsAssigneeService;

    public async Task NotifyObjectUpdatingAsync(Guid id, ObjectTypes objectType)
    {
        var clients = await ObjectsAssigneeService.GetObjectAssignedClients(id);
        foreach (var i in clients)
        {
            await DbContext.ObjectUpdates.AddAsync(new ObjectUpdate()
            {
                ObjectId = id,
                ObjectType = objectType,
                TargetCuid = i.Cuid,
                UpdateTime = DateTime.Now
            });
        }
    }
    
    public async Task NotifyClientUpdatingAsync(Guid? cuid=null, string? id=null, long? group=null)
    {
        var clients = await DbContext.Clients.Where(x =>
            (cuid != null && x.Cuid == cuid) ||
            (id != null && x.Id == id) ||
            (group != null && x.AbstractClient.GroupId == group)
        ).Select(x => x).ToListAsync();
        foreach (var i in clients)
        {
            await DbContext.ObjectUpdates.AddAsync(new ObjectUpdate()
            {
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

    public bool IsClientUpdated(string cuid, ObjectTypes type)
    {
        throw new NotImplementedException();
    }
}