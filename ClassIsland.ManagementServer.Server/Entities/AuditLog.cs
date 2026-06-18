using System.ComponentModel.DataAnnotations;
using ClassIsland.ManagementServer.Server.Abstractions.Entities;

namespace ClassIsland.ManagementServer.Server.Entities;

/// <summary>
/// 审计日志条目
/// </summary>
public class AuditLog : IObjectWithTime
{
    /// <summary>
    /// 日志 ID
    /// </summary>
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// 客户端 CUID
    /// </summary>
    public Guid ClientCuid { get; set; }

    /// <summary>
    /// 审计事件类型
    /// </summary>
    public int Event { get; set; }

    /// <summary>
    /// 事件负载（protobuf 序列化）
    /// </summary>
    public byte[]? Payload { get; set; }

    /// <summary>
    /// 客户端报告的事件时间（UTC）
    /// </summary>
    public DateTime EventTimeUtc { get; set; }

    /// <summary>
    /// 服务端接收时间
    /// </summary>
    public DateTime CreatedTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 上次修改时间
    /// </summary>
    public DateTime UpdatedTime { get; set; } = DateTime.Now;
}
