using ClassIsland.ManagementServer.Server.Abstractions.Entities;
using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.ManagementServer.Server.Enums;

namespace ClassIsland.ManagementServer.Server.Models.Assignees;

public class ClientAssigneeState<TObject>(AssigneeTypes assigneeType, TObject clientObject, ObjectsAssignee? assignee) : 
    IObjectWithTime where TObject : IObjectWithTime
{
    public AssigneeTypes AssigneeType { get; } = assigneeType;

    public TObject ClientObject { get; } = clientObject;

    public ObjectsAssignee? Assignee { get; } = assignee;
    
    public bool HasAssignee => Assignee != null;

    // 由于我们希望这部分数据发送到前端时保持和客户端列表相同的排序，
    // 所以这里采取了和客户端对象相同的时间信息。
    public DateTime CreatedTime => ClientObject.CreatedTime;
    public DateTime UpdatedTime => ClientObject.UpdatedTime;
}