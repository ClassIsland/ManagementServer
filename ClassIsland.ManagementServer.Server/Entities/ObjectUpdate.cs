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

    public virtual Client? TargetCu { get; set; }
}
