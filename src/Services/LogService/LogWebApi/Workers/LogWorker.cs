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
        private readonly ILogger<LogWorker> _logger;

        public LogWorker(IConnectionMultiplexer redis, ILogger<LogWorker> logger)
        {
            _redis = redis;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var subscriber = _redis.GetSubscriber();
            await subscriber.SubscribeAsync(RedisChannel.Literal("log_channel"), (channel, message) =>
            {
                // Log Service Subscriber içinde
                var logEntry = JsonSerializer.Deserialize<LogMessage>(message.ToString());

                switch (logEntry.Level)
                {
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

            await subscriber.SubscribeAsync(RedisChannel.Literal("event_message"), (channel, message) =>
            {
                var event_message = message.ToString();
                Console.WriteLine($"[log alındı] : {event_message}");
                _logger.LogInformation("gelen log : {log}", event_message);
            });
        }
    }
}
