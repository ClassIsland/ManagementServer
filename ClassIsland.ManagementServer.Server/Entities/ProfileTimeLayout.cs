using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ClassIsland.ManagementServer.Server.Abstractions.Entities;

namespace ClassIsland.ManagementServer.Server.Entities;

public partial class ProfileTimeLayout : IDbAttachableObject, IObjectWithTime
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; } = "";
    
    
    public Guid GroupId { get; set; } = ProfileGroup.DefaultGroupId;

    public virtual ProfileGroup? Group { get; set; }

    public virtual ICollection<ProfileClassplan> ProfileClassPlans { get; set; } = new List<ProfileClassplan>();

    public virtual ICollection<ProfileTimeLayoutTimePoint> ProfileTimelayoutTimepoints { get; set; } = new List<ProfileTimeLayoutTimePoint>();

    [Column(TypeName = "json")] public Dictionary<string, object?> AttachedObjects { get; set; } = new();
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedTime { get; set; } = DateTime.Now;
    
    /// <summary>
    /// 上次修改时间
    /// </summary>
    public DateTime UpdatedTime { get; set; } = DateTime.Now;
}
