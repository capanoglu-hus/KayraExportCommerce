namespace ProductionService.Persistence
{
    public interface ICacheService
    {
        Task<T> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan expiration);
        
        Task RemoveAsync(string key);

        Task<(bool Success, T Value)> TryGetValueAsync<T>(string key);

        Task PublishEventAsync(string channel, object message);
    }
}
