using ClassIsland.Core.Protobuf.Client;
using ClassIsland.Core.Protobuf.Enum;
using ClassIsland.Core.Protobuf.Server;
using ClassIsland.Core.Protobuf.Service;
using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using Grpc.Core;

namespace ClassIsland.ManagementServer.Server.Services.Grpc;

public class ClientRegisterService(ManagementServerContext dbContext) : ClientRegister.ClientRegisterBase
{
    private ManagementServerContext DbContext { get; } = dbContext;
    
    public override async Task<ClientRegisterScRsp> Register(ClientRegisterCsReq request, ServerCallContext context)
    {
        var result = new ClientRegisterScRsp();
        if (DbContext.Clients.Any(x => x.Cuid == request.ClientUid))
        {
            result.Retcode = Retcode.Registered;
            result.Message = "Client has already been registered";

            return result;
        }

        try
        {
            var newClient = new Client()
            {
                Cuid = request.ClientUid,
                Id = request.ClientId,
                RegisterTime = DateTime.Now
            };
            DbContext.Clients.Add(newClient);
            await DbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            result.Retcode = Retcode.ServerInternalError;
            result.Message = e.Message;
            return result;
        }

        result.Retcode = Retcode.Success;
        return result;
    }

    public override async Task<ClientRegisterScRsp> UnRegister(ClientRegisterCsReq request, ServerCallContext context)
    {
        var result = new ClientRegisterScRsp();

        if (!DbContext.Clients.Any(x => x.Cuid == request.ClientUid))
        {
            result.Retcode = Retcode.ClientNotFound;
            result.Message = "Client not found.";
            return result;
        }

        try
        {
            await foreach (var client in DbContext.Clients)
            {
                if (client.Cuid == request.ClientUid && client.Id == request.ClientId)
                {
                    DbContext.Clients.Remove(client);
                    break;
                }
            }

            result.Retcode = Retcode.Success;
            return result;
        }
        catch (Exception e)
        {
            result.Retcode = Retcode.ServerInternalError;
            result.Message = e.Message;
            return result;
        }
    }
}