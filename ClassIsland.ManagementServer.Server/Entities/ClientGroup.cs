using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ClassIsland.ManagementServer.Server.Abstractions.Entities;

namespace ClassIsland.ManagementServer.Server.Entities;

/// <summary>
/// 代表一个客户端分组
/// </summary>
public partial class ClientGroup : IObjectWithTime
{
    /// <summary>
    /// 当前客户端组 ID
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    /// <summary>
    /// 客户端组名称
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// 客户端组包含的客户端
    /// </summary>
    public virtual ICollection<AbstractClient> Clients { get; set; } = new List<AbstractClient>();

    /// <summary>
    /// 客户端组的对象分配信息
    /// </summary>
    public virtual ICollection<ObjectsAssignee> ObjectsAssignees { get; set; } = new List<ObjectsAssignee>();
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedTime { get; set; } = DateTime.Now;
    
    /// <summary>
    /// 上次修改时间
    /// </summary>
    public DateTime UpdatedTime { get; set; } = DateTime.Now;
    
    [StringLength(7)]
    public string ColorHex { get; set; } = "#66CCFF"; 
    
    public const long DefaultGroupId = 0;
    public const long GlobalGroupId = -1;
}
