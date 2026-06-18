using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.Shared.Protobuf.Client;
using ClassIsland.Shared.Protobuf.Enum;
using ClassIsland.Shared.Protobuf.Server;
using ClassIsland.Shared.Protobuf.Service;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Services.Grpc;

public class AuditService(
    ILogger<AuditService> logger,
    ManagementServerContext dbContext) : Audit.AuditBase
{
    public ILogger<AuditService> Logger { get; } = logger;
    public ManagementServerContext DbContext { get; } = dbContext;

    public override async Task<AuditScRsp> LogEvent(AuditScReq request, ServerCallContext context)
    {
        var ret = new AuditScRsp();
        try
        {
            var cuidStr = context.RequestHeaders.GetValue("cuid");
            if (string.IsNullOrEmpty(cuidStr) || !Guid.TryParse(cuidStr, out var cuid))
            {
                ret.Retcode = Retcode.InvalidRequest;
                ret.Message = "Invalid cuid";
                return ret;
            }

            var client = await DbContext.Clients.FindAsync(cuid);
            if (client == null)
            {
                ret.Retcode = Retcode.ClientNotFound;
                ret.Message = "Client not found";
                return ret;
            }

            var eventTime = request.TimestampUtc > 0
                ? DateTimeOffset.FromUnixTimeSeconds(request.TimestampUtc).UtcDateTime
                : DateTime.UtcNow;

            var log = new AuditLog
            {
                ClientCuid = cuid,
                Event = (int)request.Event,
                Payload = request.Payload?.ToByteArray(),
                EventTimeUtc = eventTime
            };

            await DbContext.AuditLogs.AddAsync(log);
            await DbContext.SaveChangesAsync();

            Logger.LogInformation("收到客户端 {Cuid} 的审计事件: {Event}", cuid, request.Event);
            ret.Retcode = Retcode.Success;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "处理审计事件时出错");
            ret.Retcode = Retcode.ServerInternalError;
            ret.Message = e.Message;
        }

        return ret;
    }
}
