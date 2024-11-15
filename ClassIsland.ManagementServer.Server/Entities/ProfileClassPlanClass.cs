using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ClassIsland.Shared.Interfaces;

namespace ClassIsland.ManagementServer.Server.Entities;

public partial class ProfileClassPlanClass : IDbAttachableObject
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
}
