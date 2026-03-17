using static AuthApi.Dtos.LogDataMessage;

namespace AuthApi.Services
{
    public interface IRedisService
    {
        Task PublishEventAsync(string channel, object message);
        Task PublishLogAsync(string channel, LogMessage message);
    }
}
