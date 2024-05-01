using System;
using System.Collections.Generic;

namespace ClassIsland.ManagementServer.Server.Entities;

public partial class ProfileClassplan
{
    public string Id { get; set; } = null!;

    public string? Name { get; set; }

    public string? GroupId { get; set; }

    public int? WeekDay { get; set; }

    public int? WeekDiv { get; set; }

    public string? TimeLayoutId { get; set; }

    public bool? IsEnabled { get; set; }

    public string? AttachedObjects { get; set; }

    public virtual ProfileGroup? Group { get; set; }

    public virtual ICollection<ProfileClassplanClass> ProfileClassplanClasses { get; set; } = new List<ProfileClassplanClass>();

    public virtual ProfileTimelayout? TimeLayout { get; set; }
}
