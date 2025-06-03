using ClassIsland.Shared.Protobuf.Client;
using ClassIsland.Shared.Protobuf.Server;
using ClassIsland.Shared.Protobuf.Service;
using Grpc.Core;

namespace ClassIsland.ManagementServer.Server.Services.Grpc;

public class AuditService : Audit.AuditBase
{
    public override async Task<AuditScRsp> LogEvent(AuditScReq request, ServerCallContext context)
    {
        // todo
        return new AuditScRsp();
    }
}