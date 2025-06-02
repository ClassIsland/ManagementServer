using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ClassIsland.ManagementServer.Server.Abstractions.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Entities;

public class AbstractClient : IObjectWithTime
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public long InternalId { get; set; }
    
    public string Id { get; set; } = "";
    
    public DateTime CreatedTime { get; set; } = DateTime.Now;
    
    public DateTime UpdatedTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 客户端分组 ID
    /// </summary>
    public long GroupId { get; set; } = 0;
    
    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();
    
    [ForeignKey(nameof(GroupId))]
    public virtual ClientGroup? Group { get; set; }
}