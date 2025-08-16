using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.Shared.Protobuf.Enum;
using ClassIsland.Shared.Protobuf.Server;
using Google.Protobuf;
using Grpc.Core;

namespace ClassIsland.ManagementServer.Server.Services;

public class ClientCommandDeliverService(ManagementServerContext managementServerContext, 
    ObjectsAssigneeService objectsAssigneeService,
    CyreneMspConnectionService cyreneMspConnectionService,
    ILogger<ClientCommandDeliverService> logger)
{
    private ObjectsAssigneeService ObjectsAssigneeService { get; } = objectsAssigneeService;
    private CyreneMspConnectionService CyreneMspConnectionService { get; } = cyreneMspConnectionService;

    private ManagementServerContext DbContext { get; } = managementServerContext;

    private ILogger<ClientCommandDeliverService> Logger { get; } = logger;

    public async Task DeliverCommandAsync(CommandTypes type, IBufferMessage payload, ObjectsAssignee target)
    {
        var clients = await ObjectsAssigneeService.GetAssignedClients(target);
        foreach (var i in clients)
        {
            Logger.LogInformation("向 {} 发送命令 {}", i.Cuid, type);
            if (!CyreneMspConnectionService.Sessions.TryGetValue(i.Cuid, out var session))
            {
                Logger.LogTrace("{} 未连接", i.Cuid);
                continue;
            }

            if (!session.IsActivated || session.CommandFlowWriter == null)
            {
                Logger.LogTrace("{} 的会话（{}）未激活", i.Cuid, session.SessionId);
                return;
            }
            

            await session.CommandFlowWriter.WriteAsync(new ClientCommandDeliverScRsp()
            {
                RetCode = Retcode.Success,
                Type = type,
                Payload = payload.ToByteString()
            });
            Logger.LogInformation("向 {} 发送命令 {} 成功", i.Cuid, type);
        }
    }
}