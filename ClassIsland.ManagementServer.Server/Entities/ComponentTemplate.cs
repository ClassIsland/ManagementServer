using System.ComponentModel.DataAnnotations;
using ClassIsland.ManagementServer.Server.Abstractions.Entities;

namespace ClassIsland.ManagementServer.Server.Entities;

/// <summary>
/// 主界面组件模板
/// </summary>
public class ComponentTemplate : IObjectWithTime
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 模板名称
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// 组件布局 JSON
    /// </summary>
    public string LayoutJson { get; set; } = "[]";

    /// <summary>
    /// 插件组件类型注册表 JSON
    /// </summary>
    public string PluginComponentTypes { get; set; } = "[]";

    public DateTime CreatedTime { get; set; } = DateTime.Now;
    public DateTime UpdatedTime { get; set; } = DateTime.Now;
}
