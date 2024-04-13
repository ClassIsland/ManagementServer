using System;
using System.Collections.Generic;

namespace ClassIsland.ManagementServer.Server.Entities;

public partial class ProfileSubject
{
    public string Id { get; set; } = null!;

    public string? GroupId { get; set; }

    public string? Name { get; set; }

    public string? Initials { get; set; }

    public sbyte? IsOutDoor { get; set; }

    public string? AttachedObjects { get; set; }

    public virtual ProfileGroup? Group { get; set; }

    public virtual ICollection<ObjectUpdate> ObjectUpdates { get; set; } = new List<ObjectUpdate>();

    public virtual ICollection<ObjectsAssignee> ObjectsAssignees { get; set; } = new List<ObjectsAssignee>();

    public virtual ICollection<ProfileClassplanClass> ProfileClassplanClasses { get; set; } = new List<ProfileClassplanClass>();
}
