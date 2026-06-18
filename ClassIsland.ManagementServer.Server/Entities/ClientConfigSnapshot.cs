using System.ComponentModel.DataAnnotations;
using ClassIsland.ManagementServer.Server.Abstractions.Entities;

namespace ClassIsland.ManagementServer.Server.Entities;

/// <summary>
/// 客户端上传的配置快照
/// </summary>
public class ClientConfigSnapshot : IObjectWithTime
{
    /// <summary>
    /// 快照 ID
    /// </summary>
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// 客户端 CUID
    /// </summary>
    public Guid ClientCuid { get; set; }

    /// <summary>
    /// 关联的请求 GUID
    /// </summary>
    public string RequestGuidId { get; set; } = "";

    /// <summary>
    /// 配置类型
    /// </summary>
    public int ConfigType { get; set; }

    /// <summary>
    /// JSON 序列化的配置数据
    /// </summary>
    public string Payload { get; set; } = "";

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 上次修改时间
    /// </summary>
    public DateTime UpdatedTime { get; set; } = DateTime.Now;
}
