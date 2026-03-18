using static ProductionService.Persistence.LogDataMessage;

namespace ProductionService.Persistence
{
    public interface ICacheService
    {
        Task<T> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan expiration);
        
        Task RemoveAsync(string key);


        Task PublishEventAsync(string channel, object message);
        Task PublishLogAsync(string channel, LogMessage message);
    }
}
