using System.ComponentModel.DataAnnotations;
using ClassIsland.ManagementServer.Server.Abstractions.Entities;

namespace ClassIsland.ManagementServer.Server.Entities;

/// <summary>
/// 客户端插件安装请求
/// </summary>
public class ClientPluginInstallRequest : IObjectWithTime
{
    [Key]
    public long Id { get; set; }

    public Guid ClientCuid { get; set; }

    /// <summary>
    /// 服务器上的文件名
    /// </summary>
    public string FileName { get; set; } = "";

    /// <summary>
    /// 服务器上的文件路径
    /// </summary>
    public string FilePath { get; set; } = "";

    /// <summary>
    /// 状态: 0=Pending, 1=Downloaded, 2=Installed, 3=Failed
    /// </summary>
    public int Status { get; set; } = 0;

    public DateTime CreatedTime { get; set; } = DateTime.Now;
    public DateTime UpdatedTime { get; set; } = DateTime.Now;
}
