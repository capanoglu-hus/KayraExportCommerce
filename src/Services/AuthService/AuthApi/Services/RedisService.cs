using AuthApi.Dtos;
using StackExchange.Redis;
using System.Text.Json;
using static AuthApi.Dtos.LogDataMessage;

namespace AuthApi.Services
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _redisDb;

        public RedisService(IConnectionMultiplexer redis)
        {
            _redisDb = redis.GetDatabase();
        }
        /* event için redis  pub.*/
        public async Task PublishEventAsync(string channel, object event_message)
        {
            var subscriber = _redisDb.Multiplexer.GetSubscriber();
            var payload = JsonSerializer.Serialize(event_message);

            await subscriber.PublishAsync(RedisChannel.Literal(channel), payload);
        }
        /* log için redis  pub.*/
        public async Task PublishLogAsync(string channel, LogMessage Log_message)
        {
            var subscriber = _redisDb.Multiplexer.GetSubscriber();
            var payload = JsonSerializer.Serialize(Log_message);

            await subscriber.PublishAsync(RedisChannel.Literal(channel), payload);
        }

       
    }
}
