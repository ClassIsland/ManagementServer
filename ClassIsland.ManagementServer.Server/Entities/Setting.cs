using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ClassIsland.ManagementServer.Server.Abstractions.Entities;

namespace ClassIsland.ManagementServer.Server.Entities;

public partial class Setting : IObjectWithTime
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    public string Settings { get; set; } = "{}";
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedTime { get; set; } = DateTime.Now;
    
    /// <summary>
    /// 上次修改时间
    /// </summary>
    public DateTime UpdatedTime { get; set; } = DateTime.Now;
}
