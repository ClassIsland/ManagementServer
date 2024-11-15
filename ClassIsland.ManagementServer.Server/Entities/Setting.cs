using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClassIsland.ManagementServer.Server.Entities;

public partial class Setting
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    public string Settings { get; set; } = "{}";
}
