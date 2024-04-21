using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.ManagementServer.Server.Enums;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Services;

public class ObjectsAssigneeService(ManagementServerContext context)
{
    private ManagementServerContext DbContext { get; } = context;

    public async Task<List<Client>> GetObjectAssignedClients(string objectId)
    {
        var r = new List<Client>();
        var assignees = DbContext.ObjectsAssignees.Where(x => x.ObjectId == objectId).Select(x => x).ToList();
        foreach (var i in assignees)
        {
            r.AddRange(await DbContext.Clients.Where(x =>
                ((i.TargetGroupId != null && i.TargetGroupId == x.GroupId) ||
                 (i.TargetClientId != null && i.TargetClientId == x.Id) ||
                 (i.TargetClientCuid != null && i.TargetClientCuid == x.Cuid)) &&
                !r.Contains(x)).Select(x => x).ToListAsync());
        }

        return r;
    }

    public async Task<List<ObjectsAssignee>> GetClientAssigningObjects(Client client, ObjectTypes type)
    {
        return await DbContext.ObjectsAssignees.Where(x => 
            x.ObjectType == (int)type &&
            ((x.TargetClientCuid != null && x.TargetClientCuid == client.Cuid) ||
             (x.TargetClientId != null && x.TargetClientId == client.Id) ||
             (x.TargetGroupId != null && x.TargetGroupId == client.GroupId))).Select(x => x).ToListAsync();
    }
}