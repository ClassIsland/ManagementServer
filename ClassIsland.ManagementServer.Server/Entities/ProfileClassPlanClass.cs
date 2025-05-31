using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using ClassIsland.ManagementServer.Server.Abstractions.Entities;
using ClassIsland.Shared.Interfaces;

namespace ClassIsland.ManagementServer.Server.Entities;

public partial class ProfileClassPlanClass : IDbAttachableObject, IObjectWithTime
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long InternalId { get; set; }

    public int Index { get; set; }

    public Guid ParentId { get; set; }

    public Guid SubjectId { get; set; }
    
    public virtual ProfileClassplan Parent { get; set; } = new();
    
    public virtual ProfileSubject Subject { get; set; } = new();

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
