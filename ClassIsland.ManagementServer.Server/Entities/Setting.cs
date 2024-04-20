using System;
using System.Collections.Generic;

namespace ClassIsland.ManagementServer.Server.Entities;

public partial class Setting
{
    public string Id { get; set; } = null!;

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Settings { get; set; }
}
