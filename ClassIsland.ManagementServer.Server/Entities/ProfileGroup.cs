using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClassIsland.ManagementServer.Server.Entities;

public partial class ProfileGroup
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    public virtual ICollection<ProfileClassplan> ProfileClassplans { get; set; } = new List<ProfileClassplan>();

    public virtual ICollection<ProfileSubject> ProfileSubjects { get; set; } = new List<ProfileSubject>();

    public virtual ICollection<ProfileTimeLayout> ProfileTimelayouts { get; set; } = new List<ProfileTimeLayout>();
}
