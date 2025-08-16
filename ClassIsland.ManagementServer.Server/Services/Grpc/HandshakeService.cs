using ClassIsland.ManagementServer.Server.Context;
using ClassIsland.Shared.Protobuf.Client;
using ClassIsland.Shared.Protobuf.Enum;
using ClassIsland.Shared.Protobuf.Server;
using ClassIsland.Shared.Protobuf.Service;
using Grpc.Core;
using PgpCore;

namespace ClassIsland.ManagementServer.Server.Services.Grpc;

public class HandshakeService(ILogger<HandshakeService> logger,
    CyreneMspConnectionService cyreneMspConnectionService,
    ManagementServerContext dbContext) : Handshake.HandshakeBase
{
    private ILogger<HandshakeService> Logger { get; } = logger;
    private CyreneMspConnectionService CyreneMspConnectionService { get; } = cyreneMspConnectionService;
    private ManagementServerContext DbContext { get; } = dbContext;

    public override async Task<HandshakeScBeginHandShakeRsp> BeginHandshake(HandshakeScBeginHandShakeReq request,
        ServerCallContext context)
    {
        Logger.LogInformation("准备与客户端 {} 握手", request.ClientUid);
        var client = await DbContext.Clients.FindAsync(Guid.Parse(request.ClientUid));
        if (client == null || !string.Equals(client.Mac, request.ClientMac, StringComparison.CurrentCultureIgnoreCase))
        {
            return new HandshakeScBeginHandShakeRsp()
            {
                Retcode = Retcode.HandshakeClientRejected,
                Message = client == null ? "找不到对应的客户端" : "客户端 MAC 地址验证失败"
            };
        }

        if (request.ChallengeTokenEncrypted.Length > 1000)
        {
            throw new RpcException(new Status(StatusCode.OutOfRange, "提供的挑战令牌太长"));
        }

        using var pgp = new PGP(CyreneMspConnectionService.ServerKey);
        var decrypted = await pgp.DecryptAsync(request.ChallengeTokenEncrypted);

        Logger.LogTrace("与客户端 {} 第一次握手成功", request.ClientUid);
        return new HandshakeScBeginHandShakeRsp()
        {
            Retcode = Retcode.Success,
            ChallengeTokenDecrypted = decrypted,
            ServerPublicKey = CyreneMspConnectionService.ServerPublicKeyArmored
        };
    }

    public override async Task<HandshakeScCompleteHandshakeRsp> CompleteHandshake(HandshakeScCompleteHandshakeReq request, ServerCallContext context)
    {
        var cuid = Guid.Parse(context.RequestHeaders.GetValue("cuid") ?? "");
        Logger.LogTrace("准备与客户端 {} 第二次", cuid);
        if (!request.Accepted)
        {
            return new HandshakeScCompleteHandshakeRsp()
            {
                Retcode = Retcode.Success
            };
        }

        if (!CyreneMspConnectionService.EstablishClientConnection(cuid, out var sessionId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "此客户端已与服务端建立连接"));
        }
        
        return new HandshakeScCompleteHandshakeRsp()
        {
            Retcode = Retcode.Success,
            SessionId = sessionId
        };
    }
}