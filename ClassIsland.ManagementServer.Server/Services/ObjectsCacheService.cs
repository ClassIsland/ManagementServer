using ClassIsland.ManagementServer.Server.ComponentModels;
using ClassIsland.ManagementServer.Server.Enums;
using ClassIsland.Shared.Models.Profile;

namespace ClassIsland.ManagementServer.Server.Services;

public class ObjectsCacheService(IConfiguration configuration, ILogger<ObjectsCacheService> logger)
{
    public IConfiguration Configuration { get; } = configuration;
    public ILogger<ObjectsCacheService> Logger { get; } = logger;
    
    
    private const int DefaultCacheCapacity = 1024;
    
    public ThreadSafeLruCache<Guid, ClassPlan> ClassPlanCache { get; } = new (int.TryParse(configuration["CacheCapacity"], out var result) ? result : DefaultCacheCapacity);
    
    public ThreadSafeLruCache<Guid, TimeLayout> TimeLayoutCache { get; } = new (int.TryParse(configuration["CacheCapacity"], out var result) ? result : DefaultCacheCapacity);
    
    public ThreadSafeLruCache<Guid, Subject> SubjectCache { get; } = new (int.TryParse(configuration["CacheCapacity"], out var result) ? result : DefaultCacheCapacity);

    public void InvalidateCache(ObjectTypes type, Guid id)
    {
        bool success;
        switch (type)
        {
            case ObjectTypes.ProfileClassPlan:
                success = ClassPlanCache.Remove(id);
                break;
            case ObjectTypes.ProfileTimeLayout:
                success = TimeLayoutCache.Remove(id);
                break;
            case ObjectTypes.ProfileSubject:
                success = SubjectCache.Remove(id);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(id));
        }

        if (success)
        {
            Logger.LogTrace("移除对象缓存 {} {}", type, id);
        }
    }
}