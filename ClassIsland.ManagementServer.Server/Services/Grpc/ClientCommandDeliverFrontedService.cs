using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.ManagementServer.Server.Enums;
using ClassIsland.Shared.Protobuf.Client;
using ClassIsland.Shared.Protobuf.Command;
using ClassIsland.Shared.Protobuf.Enum;
using ClassIsland.Shared.Protobuf.Server;
using ClassIsland.Shared.Protobuf.Service;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace ClassIsland.ManagementServer.Server.Services.Grpc;

public class ClientCommandDeliverFrontedService(ClientCommandDeliverService clientCommandDeliverService,
    ILogger<ClientCommandDeliverFrontedService> logger,
    CyreneMspConnectionService cyreneMspConnectionService) : ClientCommandDeliver.ClientCommandDeliverBase
{
    private ClientCommandDeliverService ClientCommandDeliverService { get; } = clientCommandDeliverService;

    private ILogger<ClientCommandDeliverFrontedService> Logger { get; } = logger;
    private CyreneMspConnectionService CyreneMspConnectionService { get; } = cyreneMspConnectionService;

    public override async Task ListenCommand(IAsyncStreamReader<ClientCommandDeliverScReq> requestStream, IServerStreamWriter<ClientCommandDeliverScRsp> responseStream, ServerCallContext context)
    {
        if (!Guid.TryParse(context.RequestHeaders.GetValue("cuid"), out var cuid) ||
            !CyreneMspConnectionService.Sessions.TryGetValue(cuid, out var session))
        {
            await responseStream.WriteAsync(new ClientCommandDeliverScRsp()
            {
                RetCode = Retcode.InvalidRequest
            });
            return;
        }

        session.CommandFlowWriter = responseStream;
        session.IsActivated = true;
        
        Logger.LogInformation("与 {} 建立命令流连接", cuid);
        await ClientCommandDeliverService.DeliverCommandAsync(CommandTypes.Pong, new Empty(),
            new ObjectsAssignee()
            {
                AssigneeType = AssigneeTypes.ClientUid,
                TargetClientCuid = cuid
            });
        while (!context.CancellationToken.IsCancellationRequested)
        {
            try
            {
                await requestStream.MoveNext(context.CancellationToken);
                var request = requestStream.Current;
                if (request.Type == CommandTypes.Ping)
                {
                    await responseStream.WriteAsync(new ClientCommandDeliverScRsp()
                    {
                        RetCode = Retcode.Success,
                        Type = CommandTypes.Pong,
                        Payload = new HeartBeat().ToByteString()
                    });
                    Logger.LogTrace("Ping from {}", cuid);
                }
                else
                {
                    await responseStream.WriteAsync(new ClientCommandDeliverScRsp()
                    {
                        RetCode = Retcode.InvalidRequest
                    });
                }
            }
            catch (Exception e)
            { 
                break;
            }
        }

        CyreneMspConnectionService.DestroyConnection(cuid);
        Logger.LogInformation("断开与 {} 命令流连接", cuid);
    }
}