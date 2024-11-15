using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassIsland.ManagementServer.Server.Entities;

public partial class ProfileTimeLayout : IDbAttachableObject
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; } = "";
    
    
    public Guid? GroupId { get; set; }

    public virtual ProfileGroup? Group { get; set; }

    public virtual ICollection<ProfileClassplan> ProfileClassPlans { get; set; } = new List<ProfileClassplan>();

    public virtual ICollection<ProfileTimeLayoutTimePoint> ProfileTimelayoutTimepoints { get; set; } = new List<ProfileTimeLayoutTimePoint>();

    [Column(TypeName = "json")] public Dictionary<string, object?> AttachedObjects { get; set; } = new();
}
