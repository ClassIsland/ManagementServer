﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassIsland.ManagementServer.Server.Entities;

/// <summary>
/// 代表一个 ClassIsland 客户端
/// </summary>
public partial class Client
{
    /// <summary>
    /// 客户端唯一 ID
    /// </summary>
    [Key]
    public Guid Cuid { get; set; } = Guid.Empty;

    /// <summary>
    /// 客户端自定义 ID
    /// </summary>
    public string Id { get; set; } = "";

    /// <summary>
    /// 客户端注册时间
    /// </summary>
    public DateTime RegisterTime { get; set; } = DateTime.MinValue;

    /// <summary>
    /// 客户端分组 ID
    /// </summary>
    public long GroupId { get; set; }

    /// <summary>
    /// 客户端策略版本
    /// </summary>
    public int PolicyVersion { get; set; }

    /// <summary>
    /// 客户端时间表版本
    /// </summary>
    public int TimeLayoutVersion { get; set; }

    /// <summary>
    /// 科目版本
    /// </summary>
    public int SubjectsVersion { get; set; }

    /// <summary>
    /// 默认设置版本
    /// </summary>
    public int DefaultSettingsVersion { get; set; }

    /// <summary>
    /// 课表版本
    /// </summary>
    public int ClassPlanVersion { get; set; }

    /// <summary>
    /// 客户端组
    /// </summary>
    public virtual ClientGroup Group { get; set; } = new();

    
    /// <summary>
    /// 此客户端对象更新信息
    /// </summary>
    public virtual ICollection<ObjectUpdate> ObjectUpdates { get; set; } = new List<ObjectUpdate>();

    /// <summary>
    /// 此客户端的对象分配信息
    /// </summary>
    public virtual ICollection<ObjectsAssignee> ObjectsAssignees { get; set; } = new List<ObjectsAssignee>();
}
