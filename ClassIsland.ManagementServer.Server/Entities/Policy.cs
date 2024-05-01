using System;
using System.Collections.Generic;

namespace ClassIsland.ManagementServer.Server.Entities;

public partial class Policy
{
    public string Id { get; set; } = null!;

    public string? Name { get; set; }

    public bool? IsEnabled { get; set; }

    public string? Content { get; set; }
}
