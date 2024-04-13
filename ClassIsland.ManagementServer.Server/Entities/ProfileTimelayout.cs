using System;
using System.Collections.Generic;

namespace ClassIsland.ManagementServer.Server.Entities;

public partial class ProfileTimelayout
{
    public string Id { get; set; } = null!;

    public string? Name { get; set; }

    public string? GroupId { get; set; }

    public string? AttachedObjects { get; set; }

    public virtual ProfileGroup? Group { get; set; }

    public virtual ICollection<ObjectUpdate> ObjectUpdates { get; set; } = new List<ObjectUpdate>();

    public virtual ICollection<ObjectsAssignee> ObjectsAssignees { get; set; } = new List<ObjectsAssignee>();

    public virtual ICollection<ProfileClassplan> ProfileClassplans { get; set; } = new List<ProfileClassplan>();

    public virtual ICollection<ProfileTimelayoutTimepoint> ProfileTimelayoutTimepoints { get; set; } = new List<ProfileTimelayoutTimepoint>();
}
