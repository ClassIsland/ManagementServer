using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ClassIsland.ManagementServer.Server.Abstractions.Entities;

namespace ClassIsland.ManagementServer.Server.Entities;

public partial class ProfileGroup : IObjectWithTime
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    public virtual ICollection<ProfileClassplan> ProfileClassplans { get; set; } = new List<ProfileClassplan>();

    public virtual ICollection<ProfileSubject> ProfileSubjects { get; set; } = new List<ProfileSubject>();

    public virtual ICollection<ProfileTimeLayout> ProfileTimelayouts { get; set; } = new List<ProfileTimeLayout>();
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedTime { get; set; } = DateTime.Now;
    
    /// <summary>
    /// 上次修改时间
    /// </summary>
    public DateTime UpdatedTime { get; set; } = DateTime.Now;
}
