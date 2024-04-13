using System;
using System.Collections.Generic;

namespace ClassIsland.ManagementServer.Server.Entities;

public partial class ObjectsAssignee
{
    public int Id { get; set; }

    public string? ObjectId { get; set; }

    public int? ObjectType { get; set; }

    public string? TargetClientId { get; set; }

    public string? TargetClientCuid { get; set; }

    public int? TargetGroupId { get; set; }

    public virtual Policy? Object { get; set; }

    public virtual ProfileGroup? Object1 { get; set; }

    public virtual ProfileSubject? Object2 { get; set; }

    public virtual ProfileTimelayout? Object3 { get; set; }

    public virtual Setting? Object4 { get; set; }

    public virtual ProfileClassplan? ObjectNavigation { get; set; }

    public virtual Client? TargetClientCu { get; set; }

    public virtual ClientGroup? TargetGroup { get; set; }
}
