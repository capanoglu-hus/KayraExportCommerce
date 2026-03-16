using AuthApi.Models;

namespace AuthApi.Services
{
    public interface ITokenService
    {
        string CreateAccessToken(User user, IList<string> roles);
        RefreshToken CreateRefreshToken(string ipAddress);
    }
}
