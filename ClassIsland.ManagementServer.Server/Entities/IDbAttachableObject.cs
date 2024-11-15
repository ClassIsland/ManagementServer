using System.ComponentModel.DataAnnotations.Schema;
using ClassIsland.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Entities;


public interface IDbAttachableObject
{
    [Column(TypeName = "json")]
    public Dictionary<string, object?> AttachedObjects { get; set; }
}