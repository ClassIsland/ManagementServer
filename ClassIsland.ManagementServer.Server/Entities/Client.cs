using System;
using System.Collections.Generic;

namespace ClassIsland.ManagementServer.Server.Entities;

public partial class Client
{
    public string Cuid { get; set; } = null!;

    public string? Id { get; set; }

    public DateTime? RegisterTime { get; set; }

    public int? GroupId { get; set; }

    public int? PolicyVersion { get; set; }

    public int? TimeLayoutVersion { get; set; }

    public int? SubjectsVersion { get; set; }

    public int? DefaultSettingsVersion { get; set; }

    public virtual ClientGroup? Group { get; set; }

    public virtual ICollection<ObjectUpdate> ObjectUpdates { get; set; } = new List<ObjectUpdate>();

    public virtual ICollection<ObjectsAssignee> ObjectsAssignees { get; set; } = new List<ObjectsAssignee>();
}
