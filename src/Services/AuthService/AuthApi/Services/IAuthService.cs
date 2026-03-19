using AuthApi.Dtos;
using Serilog;

namespace AuthApi.Services
{
    public interface IAuthService
    {
        Task<bool> Register(RegisterDto register,string address);
        Task<TokenResponseDto> Login(LoginDto login, string address);

        Task<TokenResponseDto> RefreshToken(TokenRequestDto request, string address);
        Task<bool> Revoke(TokenRequestDto request, string address);
    }
}
