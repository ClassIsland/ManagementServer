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

public class ClientCommandDeliverFrontedService(ClientCommandDeliverService clientCommandDeliverService, ILogger<ClientCommandDeliverFrontedService> logger) : ClientCommandDeliver.ClientCommandDeliverBase
{
    private ClientCommandDeliverService ClientCommandDeliverService { get; } = clientCommandDeliverService;

    private ILogger<ClientCommandDeliverFrontedService> Logger { get; } = logger;
    
    public override async Task ListenCommand(IAsyncStreamReader<ClientCommandDeliverScReq> requestStream, IServerStreamWriter<ClientCommandDeliverScRsp> responseStream, ServerCallContext context)
    {
        if (!Guid.TryParse(context.RequestHeaders.GetValue("cuid"), out var cuid))
        {
            await responseStream.WriteAsync(new ClientCommandDeliverScRsp()
            {
                RetCode = Retcode.InvalidRequest
            });
            return;
        }

        ClientCommandDeliverService.Streams[cuid] = responseStream;
        Logger.LogInformation("与 {} 建立命令流连接", cuid);
        await ClientCommandDeliverService.DeliverCommandAsync(CommandTypes.ServerConnected, new Empty(),
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
        ClientCommandDeliverService.Streams.Remove(ClientCommandDeliverService.Streams.FirstOrDefault(x => x.Value == responseStream).Key);
        Logger.LogInformation("断开与 {} 命令流连接", cuid);
    }
}