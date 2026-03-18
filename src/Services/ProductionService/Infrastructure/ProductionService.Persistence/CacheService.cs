using StackExchange.Redis;
using System.Text.Json;
using static ProductionService.Persistence.LogDataMessage;

namespace ProductionService.Persistence
{
    public class CacheService : ICacheService
    {
        /* redis cache işlemleri için veri türleri*/
        private readonly IDatabase _redisDb;

        public CacheService(IConnectionMultiplexer redis)
        {
            _redisDb = redis.GetDatabase();
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var result = await _redisDb.StringGetAsync(key);
            if (result.HasValue)
                return JsonSerializer.Deserialize<T>(result.ToString());

            return default;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            var serializedData = JsonSerializer.Serialize(value);
            Console.WriteLine($"Redis'e yazılan veri boyutu: {serializedData.Length} karakter.");
            await _redisDb.StringSetAsync(key, serializedData, expiration);
        }

        public async Task RemoveAsync(string key)
        {
            await _redisDb.KeyDeleteAsync(key);
        }
        public async Task PublishEventAsync(string channel, object message)
        {
            var subscriber = _redisDb.Multiplexer.GetSubscriber();
            var payload = JsonSerializer.Serialize(message);

            await subscriber.PublishAsync(RedisChannel.Literal(channel), payload);
        }

        public async Task PublishLogAsync(string channel, LogMessage message)
        {
            var subscriber = _redisDb.Multiplexer.GetSubscriber();
            var payload = JsonSerializer.Serialize(message);

            await subscriber.PublishAsync(RedisChannel.Literal(channel), payload);
        }
    }
}
