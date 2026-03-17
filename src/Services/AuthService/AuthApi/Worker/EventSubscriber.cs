using StackExchange.Redis;

namespace AuthApi.Worker
{
    public class EventSubscriber :BackgroundService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IServiceProvider _service;
        private readonly ILogger<EventSubscriber> _logger;

        public EventSubscriber(ILogger<EventSubscriber> logger, IConnectionMultiplexer redis, IServiceProvider service)
        {
            _redis = redis;
            _logger = logger;
            _service = service;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var subscriber = _redis.GetSubscriber();
            return subscriber.SubscribeAsync(RedisChannel.Literal("event_message"), (channel, message) =>
            {
                var event_message = message.ToString();
                Console.WriteLine($"[event_message] : {event_message}");
               _logger.LogInformation("gelen event_message : {event_message}", event_message);
            });
        }
    }
}
