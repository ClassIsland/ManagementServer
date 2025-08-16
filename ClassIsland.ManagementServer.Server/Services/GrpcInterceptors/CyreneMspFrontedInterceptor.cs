using Grpc.Core;
using Grpc.Core.Interceptors;

namespace ClassIsland.ManagementServer.Server.Services.GrpcInterceptors;

public class CyreneMspFrontedInterceptor(ILogger<CyreneMspFrontedInterceptor> logger,
    CyreneMspConnectionService cyreneMspConnectionService) : Interceptor
{
    private CyreneMspConnectionService CyreneMspConnectionService { get; } = cyreneMspConnectionService;
    
    private const string ProtocolName = "Cyrene_MSP";
    
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var clientProtocolName = context.RequestHeaders.GetValue("protocol_name");
        if (clientProtocolName != ProtocolName)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument,
                $"客户端使用的协议 {clientProtocolName} 与服务端协议 {ProtocolName} 不符。"));
        }

        if (!context.Method.StartsWith("/ClassIsland.Shared.Protobuf.Service.Handshake/") &&
            (!CyreneMspConnectionService.Sessions.TryGetValue(Guid.Parse(context.RequestHeaders.GetValue("cuid")!),
                 out var session) ||
             session?.SessionId != context.RequestHeaders.GetValue("session"))
            )
        {
            throw new RpcException(new Status(StatusCode.PermissionDenied, "会话无效"));
        }
        
        return await continuation(request, context);
    }
}