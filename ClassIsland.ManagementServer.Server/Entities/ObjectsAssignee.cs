using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ClassIsland.ManagementServer.Server.Enums;
using Microsoft.VisualBasic.CompilerServices;

namespace ClassIsland.ManagementServer.Server.Entities;

/// <summary>
/// 代表一个对象分配信息
/// </summary>
public partial class ObjectsAssignee
{
    /// <summary>
    /// 对象分配 ID
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    /// <summary>
    /// 要分配的对象 ID
    /// </summary>
    public Guid ObjectId { get; set; }

    /// <summary>
    /// 要分配的对象类型
    /// </summary>
    public ObjectTypes ObjectType { get; set; }

    /// <summary>
    /// 要分配到的客户端 ID
    /// </summary>
    public string? TargetClientId { get; set; }

    /// <summary>
    /// 要分配到的客户端 CUID
    /// </summary>
    public Guid? TargetClientCuid { get; set; }

    /// <summary>
    /// 要分配到的客户端分组 ID
    /// </summary>
    public long? TargetGroupId { get; set; }

    /// <summary>
    /// 分配类型
    /// </summary>
    public AssigneeTypes AssigneeType { get; set; }

    public virtual Client? TargetClientCu { get; set; }

    public virtual ClientGroup? TargetGroup { get; set; }
}
