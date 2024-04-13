using System;
using System.Collections.Generic;

namespace ClassIsland.ManagementServer.Server.Entities;

public partial class ObjectUpdate
{
    public int Id { get; set; }

    public string? ObjectId { get; set; }

    public int? ObjectType { get; set; }

    public string? TargetCuid { get; set; }

    public DateTime? UpdateTime { get; set; }

    public virtual Policy? Object { get; set; }

    public virtual ProfileSubject? Object1 { get; set; }

    public virtual ProfileTimelayout? Object2 { get; set; }

    public virtual Setting? Object3 { get; set; }

    public virtual ProfileClassplan? ObjectNavigation { get; set; }

    public virtual Client? TargetCu { get; set; }
}
