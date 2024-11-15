using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassIsland.ManagementServer.Server.Entities;

public partial class ProfileTimeLayoutTimePoint : IDbAttachableObject
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long InternalId { get; set; }

    public Guid ParentId { get; set; }

    public int Index { get; set; }

    public TimeOnly Start { get; set; }

    public TimeOnly End { get; set; }

    public int TimeType { get; set; }

    public string? DefaultSubjectId { get; set; }


    public bool IsHideDefault { get; set; }

    public virtual ProfileTimeLayout Parent { get; set; } = new();
    
    [Column(TypeName = "json")] public Dictionary<string, object?> AttachedObjects { get; set; } = new();
}
