using System;
using System.Collections.Generic;

namespace ClassIsland.ManagementServer.Server.Entities;

public partial class ProfileSubject
{
    public string Id { get; set; } = null!;

    public string? GroupId { get; set; }

    public string? Name { get; set; }

    public string? Initials { get; set; }

    public bool? IsOutDoor { get; set; }

    public string? AttachedObjects { get; set; }

    public virtual ProfileGroup? Group { get; set; }

    public virtual ICollection<ProfileClassplanClass> ProfileClassplanClasses { get; set; } = new List<ProfileClassplanClass>();
}
