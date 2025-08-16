using ClassIsland.Shared.Protobuf.Server;
using Grpc.Core;

namespace ClassIsland.ManagementServer.Server.Models.CyreneMsp;

public class ClientSession
{
    public IServerStreamWriter<ClientCommandDeliverScRsp>? CommandFlowWriter { get; set; }
    
    public Guid ClientUid { get; set; }

    public string SessionId { get; set; } = "";

    public bool IsActivated { get; set; } = false;
    
    public DateTime InvalidateTime { get; set; }
}