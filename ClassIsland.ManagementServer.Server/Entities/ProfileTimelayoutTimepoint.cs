using System;
using System.Collections.Generic;

namespace ClassIsland.ManagementServer.Server.Entities;

public partial class ProfileTimelayoutTimepoint
{
    public int InternalId { get; set; }

    public string? ParentId { get; set; }

    public int? Index { get; set; }

    public TimeOnly? Start { get; set; }

    public TimeOnly? End { get; set; }

    public int? TimeType { get; set; }

    public string? DefaultSubjectId { get; set; }

    public string? AttachedObjects { get; set; }

    public virtual ProfileTimelayout? Parent { get; set; }
}
