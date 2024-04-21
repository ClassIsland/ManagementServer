using ClassIsland.Core.Protobuf.Management;
using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using Grpc.Core;

namespace ClassIsland.ManagementServer.Server.Services.Grpc;

public class ManagementServerConnectionService(ManagementServerContext dbContext) : ManagementServerConnection.ManagementServerConnectionBase
{
    private ManagementServerContext DbContext { get; } = dbContext;
    
    public override async Task<ClientRegisterResult> Register(ClientRegisterInfo request, ServerCallContext context)
    {
        if (DbContext.Clients.Any(x => x.Cuid == request.ClientUid))
        {
            return new ClientRegisterResult();
        }

        var newClient = new Client()
        {
            Cuid = request.ClientUid,
            Id = request.Id,
            RegisterTime = DateTime.Now
        };
        DbContext.Clients.Add(newClient);
        await DbContext.SaveChangesAsync();
        return new ClientRegisterResult();
    }
}