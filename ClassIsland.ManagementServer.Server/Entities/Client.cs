using System;
using System.Collections.Generic;

namespace ClassIsland.ManagementServer.Server.Entities;

public partial class Client
{
    public string Cuid { get; set; } = null!;

    public string? Id { get; set; }

    public DateTime? RegisterTime { get; set; }

    public int? GroupId { get; set; }
}
