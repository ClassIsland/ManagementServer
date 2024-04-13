using System;
using System.Collections.Generic;

namespace ClassIsland.ManagementServer.Server.Entities;

public partial class ProfileGroup
{
    public string Id { get; set; } = null!;

    public string? Name { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<ObjectsAssignee> ObjectsAssignees { get; set; } = new List<ObjectsAssignee>();

    public virtual ICollection<ProfileClassplan> ProfileClassplans { get; set; } = new List<ProfileClassplan>();

    public virtual ICollection<ProfileSubject> ProfileSubjects { get; set; } = new List<ProfileSubject>();

    public virtual ICollection<ProfileTimelayout> ProfileTimelayouts { get; set; } = new List<ProfileTimelayout>();
}
