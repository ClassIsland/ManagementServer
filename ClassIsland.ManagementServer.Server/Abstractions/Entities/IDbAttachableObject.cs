using System.ComponentModel.DataAnnotations.Schema;

namespace ClassIsland.ManagementServer.Server.Abstractions.Entities;


public interface IDbAttachableObject
{
    [Column(TypeName = "json")]
    public Dictionary<string, object?> AttachedObjects { get; set; }
}