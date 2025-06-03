using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ClassIsland.ManagementServer.Server.Abstractions.Entities;
using ClassIsland.Shared.Interfaces;

namespace ClassIsland.ManagementServer.Server.Entities;

/// <summary>
/// 代表一个档案的课表
/// </summary>
public partial class ProfileClassplan : IDbAttachableObject, IObjectWithTime
{
    /// <summary>
    /// 课表 ID
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// 课表名称
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// 课表所属分组 ID
    /// </summary>
    public Guid GroupId { get; set; } = ProfileGroup.DefaultGroupId;

    /// <summary>
    /// 在一周中的哪一天启用这个课表
    /// </summary>
    public int WeekDay { get; set; } = 0;

    /// <summary>
    /// 在多周轮换中的哪一周启用这个课表
    /// </summary>
    public int WeekDiv { get; set; }

    /// <summary>
    /// 课表所使用的时间表 ID
    /// </summary>
    public Guid TimeLayoutId { get; set; }
    
    /// <summary>
    /// 课表是否自动启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    public virtual ProfileGroup Group { get; set; } = new();

    public virtual ICollection<ProfileClassPlanClass> ProfileClassPlanClasses { get; set; } = new List<ProfileClassPlanClass>();

    public virtual ProfileTimeLayout TimeLayout { get; set; } = new();
    public Dictionary<string, object?> AttachedObjects { get; set; } = new();
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedTime { get; set; } = DateTime.Now;
    
    /// <summary>
    /// 上次修改时间
    /// </summary>
    public DateTime UpdatedTime { get; set; } = DateTime.Now;
}
