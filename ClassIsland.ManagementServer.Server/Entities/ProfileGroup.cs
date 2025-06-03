using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ClassIsland.ManagementServer.Server.Abstractions.Entities;

namespace ClassIsland.ManagementServer.Server.Entities;

public partial class ProfileGroup : IObjectWithTime
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public string Description { get; set; } = "";
    
    [StringLength(7)]
    public string ColorHex { get; set; } = "#66CCFF";
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedTime { get; set; } = DateTime.Now;
    
    /// <summary>
    /// 上次修改时间
    /// </summary>
    public DateTime UpdatedTime { get; set; } = DateTime.Now;

    public static readonly Guid DefaultGroupId = new Guid("00000000-0000-0000-0000-000000000001");
}
