using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassIsland.ManagementServer.Server.Entities;

public partial class ProfileSubject : IDbAttachableObject
{
    [Key]
    public Guid Id { get; set; }

    public Guid? GroupId { get; set; }

    public string Name { get; set; } = "";

    public string Initials { get; set; } = "";

    public bool IsOutDoor { get; set; }

    public virtual ProfileGroup? Group { get; set; }

    public virtual ICollection<ProfileClassPlanClass> ProfileClassplanClasses { get; set; } = new List<ProfileClassPlanClass>();
    
    [Column(TypeName = "json")] public Dictionary<string, object?> AttachedObjects { get; set; } = new();
}
