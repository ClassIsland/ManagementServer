namespace ClassIsland.ManagementServer.Server.Abstractions.Entities;

public interface IObjectWithTime
{
    /// <summary>
    /// 对象创建时间
    /// </summary>
    public DateTime CreatedTime { get; set; }
    
    /// <summary>
    /// 对象上次修改时间
    /// </summary>
    public DateTime UpdatedTime { get; set; }
}