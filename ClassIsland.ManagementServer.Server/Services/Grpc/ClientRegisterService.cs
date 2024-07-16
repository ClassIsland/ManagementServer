using ClassIsland.Shared.IPC.Protobuf.Client;
using ClassIsland.Shared.IPC.Protobuf.Enum;
using ClassIsland.Shared.IPC.Protobuf.Server;
using ClassIsland.Shared.IPC.Protobuf.Service;
using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Services.Grpc;

public class ClientRegisterService(ManagementServerContext dbContext) : ClientRegister.ClientRegisterBase
{
    private ManagementServerContext DbContext { get; } = dbContext;
    
    public override async Task<ClientRegisterScRsp> Register(ClientRegisterCsReq request, ServerCallContext context)
    {
        var result = new ClientRegisterScRsp();
        try
        {
            if (await DbContext.Clients.AnyAsync(x => x.Cuid == request.ClientUid))
            {
                result.Retcode = Retcode.Registered;
                result.Message = "Client has already been registered";
                return result;
            }
            var newClient = new Client()
            {
                Cuid = request.ClientUid,
                Id = request.ClientId,
                RegisterTime = DateTime.Now
            };
            await DbContext.Clients.AddAsync(newClient);
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
        try
        {
            var c = DbContext.Clients.FirstOrDefault(client => client.Cuid == request.ClientUid);
            if (c == null)
            {
                result.Retcode = Retcode.ClientNotFound;
                result.Message = "Client not found.";
                return result;
            }
            DbContext.Clients.Remove(c);
            await DbContext.SaveChangesAsync();

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
