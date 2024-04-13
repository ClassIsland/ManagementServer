using System;
using System.Collections.Generic;

namespace ClassIsland.ManagementServer.Server.Entities;

public partial class ProfileClassplanClass
{
    public int InternalId { get; set; }

    public int? Index { get; set; }

    public string? ParentId { get; set; }

    public string? SubjectId { get; set; }

    public string? AttachedObjects { get; set; }

    public virtual ProfileClassplan? Parent { get; set; }

    public virtual ProfileSubject? Subject { get; set; }
}
