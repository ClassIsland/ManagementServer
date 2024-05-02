using System.Security.Cryptography.X509Certificates;
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
                ((i.AssigneeType == (int)AssigneeTypes.Group && i.TargetGroupId == x.GroupId) ||
                 (i.AssigneeType == (int)AssigneeTypes.Id && i.TargetClientId == x.Id) ||
                 (i.AssigneeType == (int)AssigneeTypes.ClientUid && i.TargetClientCuid == x.Cuid)) &&
                !r.Contains(x)).Select(x => x).ToListAsync());
        }

        return r;
    }

    public async Task<List<ObjectsAssignee>> GetClientAssigningObjects(Client client, ObjectTypes type)
    {
        return await DbContext.ObjectsAssignees.Where(x => 
            x.ObjectType == (int)type &&
            ((x.AssigneeType == (int)AssigneeTypes.ClientUid && x.TargetClientCuid == client.Cuid) ||
             (x.AssigneeType == (int)AssigneeTypes.Id && x.TargetClientId == client.Id) ||
             (x.AssigneeType == (int)AssigneeTypes.Group && x.TargetGroupId == client.GroupId))).Select(x => x).ToListAsync();
    }

    public async Task<List<ObjectsAssignee>> GetClientAssigningObjectsLeveled(Client client, ObjectTypes type)
    {
        return await DbContext.ObjectsAssignees.Where(x => 
            x.ObjectType == (int)type &&
            ((x.AssigneeType == (int)AssigneeTypes.ClientUid && x.TargetClientCuid == client.Cuid) ||
             (x.AssigneeType == (int)AssigneeTypes.Id && x.TargetClientId == client.Id) ||
             (x.AssigneeType == (int)AssigneeTypes.Group && x.TargetGroupId == client.GroupId)))
            .OrderBy(x => x.AssigneeType ?? 0)
            .Select(x => x)
            .ToListAsync();
    }
}