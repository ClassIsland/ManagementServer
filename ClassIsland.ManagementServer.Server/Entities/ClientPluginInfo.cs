using System.ComponentModel.DataAnnotations;
using ClassIsland.ManagementServer.Server.Abstractions.Entities;

namespace ClassIsland.ManagementServer.Server.Entities;

/// <summary>
/// 客户端插件信息（由客户端上报）
/// </summary>
public class ClientPluginInfo : IObjectWithTime
{
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// 客户端 CUID
    /// </summary>
    public Guid ClientCuid { get; set; }

    /// <summary>
    /// 插件 ID
    /// </summary>
    public string PluginId { get; set; } = "";

    /// <summary>
    /// 插件名称
    /// </summary>
    public string PluginName { get; set; } = "";

    /// <summary>
    /// 插件版本
    /// </summary>
    public string Version { get; set; } = "";

    /// <summary>
    /// 插件描述
    /// </summary>
    public string Description { get; set; } = "";

    /// <summary>
    /// 作者
    /// </summary>
    public string Author { get; set; } = "";

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 加载状态: 0=NotLoaded, 1=Loaded, 2=Disabled, 3=Error
    /// </summary>
    public int LoadStatus { get; set; } = 0;

    /// <summary>
    /// 上次上报时间
    /// </summary>
    public DateTime CreatedTime { get; set; } = DateTime.Now;

    public DateTime UpdatedTime { get; set; } = DateTime.Now;
}
