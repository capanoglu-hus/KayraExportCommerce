using AuthApi.Data;
using AuthApi.Dtos;
using AuthApi.Models;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static AuthApi.Dtos.LogDataMessage;

namespace AuthApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly UserDbContext _context;
        private readonly ITokenService _tokenService;

        private readonly IRedisService _redisService;

        public AuthService(IRedisService redisService, UserManager<User> userManager, UserDbContext context, ITokenService tokenService)
        {
            _userManager = userManager;
            _context = context;
            _tokenService = tokenService;
            _redisService = redisService;
        }
        public async Task<TokenResponseDto> Login(LoginDto login ,string address)
        {
            var user = await _userManager.Users
             .Include(u => u.RefreshTokens)
             .SingleOrDefaultAsync(u => u.Email == login.Email);
            /* ilgili kullanıcıyı email üzerinden buluyor */
            if (user == null || !await _userManager.CheckPasswordAsync(user, login.Password))
            {
                /* kullanıcı olup olmadığına ve pass kontrolu yapıyor*/
                await _redisService.PublishLogAsync("log_channel", new LogMessage
                {
                    ServiceName = "Auth",
                    Level = (LogDataMessage.LogLevel)2,
                    Message = "Kullanıcı yanlış giriş",
                });
                return null;

            }
            var roles = await _userManager.GetRolesAsync(user);
            // kullnaıcının rolunu alıyor
            var accessToken = _tokenService.CreateAccessToken(user, roles);
            // yeni jwt token  
            var refreshToken = _tokenService.CreateRefreshToken(address);

            // kullnaıcıların bilgilerini veritabanına yazma
            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);
            await _redisService.PublishEventAsync("event_message", new EventMessage
            {
                Service = "AuthService/AuthController",
                Action = "login işlemi yapıldı",
                Timestamp = DateTime.UtcNow
            });

            var tokenResponse = new TokenResponseDto { AccessToken = accessToken, RefreshToken = refreshToken.Token };
            return tokenResponse;

        }

        public async Task<TokenResponseDto> RefreshToken(TokenRequestDto request,string address)
        {
            var refreshToken = request.RefreshToken;
            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken));
            /* o tokenı kullanan kullanıcıyı buluyor*/
            if (user == null)
            {
                await _redisService.PublishLogAsync("log_channel", new LogMessage
                {
                    ServiceName = "Auth",
                    Level = LogDataMessage.LogLevel.Critical,
                    Message = "geçersiz refresh token",
                });
                return null;
            }
            var existingToken = user.RefreshTokens.Single(t => t.Token == refreshToken);
            if (!existingToken.isActive)
            {
                await _redisService.PublishLogAsync("log_channel", new LogMessage
                {
                    ServiceName = "Auth",
                    Level = LogDataMessage.LogLevel.Critical,
                    Message = "refresh token aktif değil",
                });
                return null;
            }
            /*refresh tokenı iptal ediyor*/
            existingToken.Revoked = DateTime.UtcNow;
            existingToken.RevokedByIp = address;
            /* yenisini oluşturup veritabanına ekliyor */
            var newRefreshToken = _tokenService.CreateRefreshToken(address);
            existingToken.ReplacedByToken = newRefreshToken.Token;
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);
            /*yeni access token */ 
            var roles = await _userManager.GetRolesAsync(user);
            var newAccessToken = _tokenService.CreateAccessToken(user, roles);
            var token = new TokenResponseDto { AccessToken = newAccessToken, RefreshToken = newRefreshToken.Token };
            return token;

        }

        public async Task<bool> Register(RegisterDto register,string address)
        {
            var user = new User
            {
                UserName = register.UserName,
                Email = register.Email,
            };

            var result = await _userManager.CreateAsync(user, register.Password);
            /* yeni kullnaıcı ıdentity ile usermanager ekliyor*/
            if (!result.Succeeded)
            {
                await _redisService.PublishLogAsync("log_channel", new LogMessage
                {
                    ServiceName = "Auth",
                    Level = LogDataMessage.LogLevel.Critical,
                    Message = "Kullanıcı oluşturalamadı",
                });
                return false;
            }
            await _redisService.PublishEventAsync("event_message", new EventMessage
            {
                Service = "AuthService/AuthController",
                Action = "register işlemi yapıldı",
                Timestamp = DateTime.UtcNow
            });
            return true;
        }

        public async Task<bool> Revoke(TokenRequestDto request ,string address)
        {
            /*oturum kapatma*/
            var token = request.RefreshToken;
            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));
            if (user == null)
            {
                await _redisService.PublishLogAsync("log_channel", new LogMessage
                {
                    ServiceName = "Auth",
                    Level = LogDataMessage.LogLevel.Critical,
                    Message = "oturum kapatma için kullanıcı bulunamadı",
                });
                return false;
            }
            var existing = user.RefreshTokens.Single(t => t.Token == token);
            if (!existing.isActive)
            {
                await _redisService.PublishLogAsync("log_channel", new LogMessage
                {
                    ServiceName = "Auth",
                    Level = LogDataMessage.LogLevel.Critical,
                    Message = "oturum kapatma için kullanıcı bulunamadı",
                });
                return false;
            }
            existing.Revoked = DateTime.UtcNow;
            existing.RevokedByIp = address;
            await _userManager.UpdateAsync(user);
            await _redisService.PublishEventAsync("event_message", new EventMessage
            {
                Service = "AuthService/AuthController",
                Action = "revoke işlemi yapıldı",
                Timestamp = DateTime.UtcNow
            });
            return true;
        }

       
    }
}
