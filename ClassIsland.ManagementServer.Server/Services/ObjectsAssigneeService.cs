using System.Security.Cryptography.X509Certificates;
using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.ManagementServer.Server.Enums;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Services;

public class ObjectsAssigneeService(ManagementServerContext context)
{
    private ManagementServerContext DbContext { get; } = context;

    public async Task<List<Client>> GetObjectAssignedClients(Guid objectId)
    {
        var r = new List<Client>();
        var assignees = DbContext.ObjectsAssignees.Where(x => x.ObjectId == objectId).Select(x => x).ToList();
        foreach (var i in assignees)
        {
            r.AddRange(from x in await GetAssignedClients(i) where !r.Contains(x) select x);
        }

        return r;
    }

    public async Task<List<Client>> GetAssignedClients(ObjectsAssignee assignee)
    {
        if (assignee.TargetGroupId == ClientGroup.GlobalGroupId)
        {
            return await DbContext.Clients.Select(x => x)
                .ToListAsync();
        }
        return await DbContext.Clients.Where(x =>
                ((assignee.AssigneeType == AssigneeTypes.Group && assignee.TargetGroupId == x.AbstractClient.GroupId) ||
                 (assignee.AssigneeType == AssigneeTypes.Id && assignee.TargetClientId == x.Id) ||
                 (assignee.AssigneeType == AssigneeTypes.ClientUid && assignee.TargetClientCuid == x.Cuid)))
            .Select(x => x).ToListAsync();
    }

    public async Task<List<ObjectsAssignee>> GetClientAssigningObjects(Client client, ObjectTypes type)
    {
        return await BuildClientAssigneeQuery(client, type)
            .Select(x => x)
            .ToListAsync();
    }

    public async Task<List<ObjectsAssignee>> GetClientAssigningObjectsLeveled(Client client, ObjectTypes type)
    {
        return await BuildClientAssigneeQuery(client, type)
            .OrderBy(x => (int)x.AssigneeType)
            .Select(x => x)
            .ToListAsync();
    }

    private IQueryable<ObjectsAssignee> BuildClientAssigneeQuery(Client client, ObjectTypes type) =>
        DbContext.ObjectsAssignees.Where(x =>
            x.ObjectType == type &&
            ((x.AssigneeType == AssigneeTypes.Group && x.TargetGroupId == ClientGroup.GlobalGroupId) ||
             (x.AssigneeType == AssigneeTypes.ClientUid && x.TargetClientCuid == client.Cuid) ||
             (x.AssigneeType == AssigneeTypes.Id && x.TargetClientId == client.Id) ||
             (x.AssigneeType == AssigneeTypes.Group && x.TargetGroupId == client.AbstractClient.GroupId)));
}