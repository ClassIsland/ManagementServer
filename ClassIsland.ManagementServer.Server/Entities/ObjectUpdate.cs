using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ClassIsland.ManagementServer.Server.Enums;
using Microsoft.VisualBasic.CompilerServices;

namespace ClassIsland.ManagementServer.Server.Entities;

/// <summary>
/// 代表客户端对象更新信息
/// </summary>
public partial class ObjectUpdate
{
    /// <summary>
    /// 对象更新信息 ID
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    /// <summary>
    /// 要更新的对象 ID
    /// </summary>
    public Guid ObjectId { get; set; }

    /// <summary>
    /// 要更新的对象类型
    /// </summary>
    public ObjectTypes ObjectType { get; set; }

    /// <summary>
    /// 要更新的客户端 CUID
    /// </summary>
    public Guid TargetCuid { get; set; }

    /// <summary>
    /// 对象更新时间
    /// </summary>
    public DateTime UpdateTime { get; set; }

    /// <summary>
    /// 要更新的客户端
    /// </summary>
    public virtual Client TargetClient { get; set; } = new();
}
