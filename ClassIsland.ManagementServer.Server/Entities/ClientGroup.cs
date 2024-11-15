using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassIsland.ManagementServer.Server.Entities;

/// <summary>
/// 代表一个客户端分组
/// </summary>
public partial class ClientGroup
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
    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();

    /// <summary>
    /// 客户端组的对象分配信息
    /// </summary>
    public virtual ICollection<ObjectsAssignee> ObjectsAssignees { get; set; } = new List<ObjectsAssignee>();
}
