using Grpc.Core;
using Grpc.Core.Interceptors;

namespace ClassIsland.ManagementServer.Server.Services.GrpcInterceptors;

public class CyreneMspFrontedInterceptor(ILogger<CyreneMspFrontedInterceptor> logger) : Interceptor
{
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
        
        return await continuation(request, context);
    }
}