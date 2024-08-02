using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.Shared.Protobuf.Enum;
using ClassIsland.Shared.Protobuf.Server;
using Google.Protobuf;
using Grpc.Core;

namespace ClassIsland.ManagementServer.Server.Services;

public class ClientCommandDeliverService(ManagementServerContext managementServerContext, 
    ObjectsAssigneeService objectsAssigneeService,
    ILogger<ClientCommandDeliverService> logger)
{
    public static Dictionary<string, IServerStreamWriter<ClientCommandDeliverScRsp>> Streams { get; } = new();

    private ObjectsAssigneeService ObjectsAssigneeService { get; } = objectsAssigneeService;

    private ManagementServerContext DbContext { get; } = managementServerContext;

    private ILogger<ClientCommandDeliverService> Logger { get; } = logger;

    public async Task DeliverCommandAsync(CommandTypes type, IBufferMessage payload, ObjectsAssignee target)
    {
        var clients = await ObjectsAssigneeService.GetAssignedClients(target);
        foreach (var i in clients)
        {
            Logger.LogInformation("向 {} 发送命令 {}", i.Cuid, type);
            if (!Streams.ContainsKey(i.Cuid))
            {
                Logger.LogTrace("{} 未连接", i.Cuid);
                continue;
            }

            var stream = Streams[i.Cuid];
            await stream.WriteAsync(new ClientCommandDeliverScRsp()
            {
                RetCode = Retcode.Success,
                Type = type,
                Payload = payload.ToByteString()
            });
            Logger.LogInformation("向 {} 发送命令 {} 成功", i.Cuid, type);
        }
    }
}