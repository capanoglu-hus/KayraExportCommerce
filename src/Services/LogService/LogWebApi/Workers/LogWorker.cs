using LogWebApi.Dto;
using StackExchange.Redis;
using System.Text.Json;
using static LogWebApi.Dto.LogDataMessage;

namespace LogService.Workers
{
    /*redisten gelecek log ve eventler için kullanılacak*/
    public class LogWorker : BackgroundService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IServiceProvider _service;
        private readonly ILogger<LogWorker> _logger;

        public LogWorker(IServiceProvider service, IConnectionMultiplexer redis, ILogger<LogWorker> logger)
        {
            _redis = redis;
            _service = service;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {   
            
            /* dinleme başlatması */
            var subscriber = _redis.GetSubscriber();

            /* log_channel -> kanalını dinleme */
            await subscriber.SubscribeAsync(RedisChannel.Literal("log_channel"), async (channel, message) =>
            {
                // Log Service Subscriber içinde
                var logEntry = JsonSerializer.Deserialize<LogMessage>(message.ToString());
              
                switch (logEntry.Level)
                {
                    /* logger seviyelrine göre */
                    case LogDataMessage.LogLevel.Information:
                        _logger.LogInformation("{Service}: {Msg}", logEntry.ServiceName, logEntry.Message);
                        break;
                    case LogDataMessage.LogLevel.Warning:
                        _logger.LogWarning("{Service}: {Msg}", logEntry.ServiceName, logEntry.Message);
                        break;
                    case LogDataMessage.LogLevel.Error:
                        _logger.LogError("{Service}: {Msg} - Ex: {Ex}", logEntry.ServiceName, logEntry.Message, logEntry.Exception);
                        break;
                    case LogDataMessage.LogLevel.Critical:
                        _logger.LogCritical("{Service}: {Msg}", logEntry.ServiceName, logEntry.Message);
                        break;
                    case LogDataMessage.LogLevel.Debug:
                        _logger.LogDebug("{Service}: {Msg}", logEntry.ServiceName, logEntry.Message);
                        break;
                }
            
            });
            /* event_message -> kanalını dinleme */
            await subscriber.SubscribeAsync(RedisChannel.Literal("event_message"), (channel, message) =>
            {
                var event_message = message.ToString();
                _logger.LogInformation(" EVENT : {event_message}", event_message);
            });
        }
    }
}
