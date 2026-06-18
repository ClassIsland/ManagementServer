using System.ComponentModel.DataAnnotations;
using ClassIsland.ManagementServer.Server.Abstractions.Entities;

namespace ClassIsland.ManagementServer.Server.Entities;

/// <summary>
/// 客户端组件配置
/// </summary>
public class ClientComponentConfig : IObjectWithTime
{
    [Key]
    public Guid ClientCuid { get; set; }

    /// <summary>
    /// 关联的模板 ID
    /// </summary>
    public Guid? TemplateId { get; set; }

    /// <summary>
    /// 当前生效的组件布局 JSON
    /// </summary>
    public string LayoutJson { get; set; } = "[]";

    /// <summary>
    /// 客户端覆盖的配置 JSON（优先于模板）
    /// </summary>
    public string? OverrideJson { get; set; }

    /// <summary>
    /// 上报的插件组件类型 JSON
    /// </summary>
    public string? PluginComponentTypes { get; set; }

    public DateTime CreatedTime { get; set; } = DateTime.Now;
    public DateTime UpdatedTime { get; set; } = DateTime.Now;
}
