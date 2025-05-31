using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ClassIsland.ManagementServer.Server.Abstractions.Entities;

namespace ClassIsland.ManagementServer.Server.Entities;

/// <summary>
/// 代表一个客户端策略数据库信息
/// </summary>
public partial class Policy : IObjectWithTime
{
    /// <summary>
    /// 策略数据库 ID
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// 策略名称
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// 策略是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 策略对象
    /// </summary>
    public ClassIsland.Shared.Models.Management.ManagementPolicy Content { get; set; } = new();
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedTime { get; set; } = DateTime.Now;
    
    /// <summary>
    /// 上次修改时间
    /// </summary>
    public DateTime UpdatedTime { get; set; } = DateTime.Now;
}
