using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;

namespace ClassIsland.ManagementServer.Server.Services;

public class ObjectsAssigneeService(ManagementServerContext context)
{
    private ManagementServerContext DbContext { get; } = context;

    public List<Client> GetObjectAssignedClients(string objectId)
    {
        var r = new List<Client>();
        var assignees = DbContext.ObjectsAssignees.Where(x => x.ObjectId == objectId).Select(x => x);
        foreach (var i in assignees)
        {
            r.AddRange(DbContext.Clients.Where(x =>
                (i.TargetGroupId == x.GroupId || i.TargetClientId == x.Id || i.TargetClientCuid == x.Cuid) &&
                !r.Contains(x)).Select(x => x));
        }

        return r;
    }
}