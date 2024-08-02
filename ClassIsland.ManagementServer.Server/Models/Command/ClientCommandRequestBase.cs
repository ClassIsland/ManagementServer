using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.Shared.Protobuf.Enum;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

namespace ClassIsland.ManagementServer.Server.Models.Command;

public abstract class ClientCommandRequestBase
{
    public List<ObjectsAssignee> Targets { get; set; } = new();

    public CommandTypes Type { get; set; } = CommandTypes.DefaultCommand;
}