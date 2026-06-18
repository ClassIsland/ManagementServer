using System.Collections.Concurrent;
using ClassIsland.Shared.Protobuf.Enum;

namespace ClassIsland.ManagementServer.Server.Services;

/// <summary>
/// 跟踪待处理的 GetClientConfig 请求，用于将上传的配置路由到正确的表
/// </summary>
public class PendingConfigRequestService
{
    private readonly ConcurrentDictionary<string, PendingRequest> _pending = new();

    public void TrackRequest(string requestGuid, ConfigTypes configType, Guid clientCuid)
    {
        _pending[requestGuid] = new PendingRequest
        {
            ConfigType = configType,
            ClientCuid = clientCuid,
            CreatedAt = DateTime.Now
        };
    }

    public PendingRequest? ConsumeRequest(string requestGuid)
    {
        _pending.TryRemove(requestGuid, out var request);
        return request;
    }

    public class PendingRequest
    {
        public ConfigTypes ConfigType { get; set; }
        public Guid ClientCuid { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
