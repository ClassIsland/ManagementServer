using ClassIsland.Core.Protobuf.Command;
using Google.Protobuf;

namespace ClassIsland.ManagementServer.Server.Models.Command;

public class SendNotificationRequest : ClientCommandRequestBase
{
    public SendNotification Payload { get; set; } = new();
}