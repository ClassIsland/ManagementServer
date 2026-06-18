using System.ComponentModel.DataAnnotations;
using ClassIsland.ManagementServer.Server.Abstractions.Entities;

namespace ClassIsland.ManagementServer.Server.Entities;

/// <summary>
/// 客户端自动化配置
/// </summary>
public class ClientAutomationConfig : IObjectWithTime
{
    [Key]
    public Guid ClientCuid { get; set; }

    /// <summary>
    /// 自动化工作流 JSON
    /// </summary>
    public string WorkflowsJson { get; set; } = "[]";

    public DateTime CreatedTime { get; set; } = DateTime.Now;
    public DateTime UpdatedTime { get; set; } = DateTime.Now;
}
