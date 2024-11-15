using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClassIsland.ManagementServer.Server.Entities;

/// <summary>
/// 代表一个客户端策略数据库信息
/// </summary>
public partial class Policy
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
}
