using AuthApi.Dtos;
using AuthApi.Models;
using Microsoft.AspNetCore.Identity;
using static AuthApi.Dtos.LogDataMessage;

namespace AuthApi.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IRedisService _redisService;


        public RoleService(IRedisService redisService, RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _redisService = redisService;
        }
        public async Task<string> AssignRole(AssignRoleDto assignRole)
        {
            var user = await _userManager.FindByEmailAsync(assignRole.Email);
            if (user == null)
            {
                
                await _redisService.PublishLogAsync("log_channel", new LogMessage
                {
                    ServiceName = "Auth / ROLE",
                    Level = LogDataMessage.LogLevel.Information,
                    Message = "geçersiz refresh token",
                });
                return ($" Bu '{assignRole.Email}' ile kullanıcı bulunamadı");
            }
            /* email ile role eşliyor*/
            var result = await _userManager.AddToRoleAsync(user, assignRole.RoleName);
            if (result == null)
            {
                await _redisService.PublishLogAsync("log_channel", new LogMessage
                {
                    ServiceName = "Auth / ROLE",
                    Level = LogDataMessage.LogLevel.Warning,
                    Message = "geçersiz refresh token",
                });
                return ($" Bu '{assignRole.RoleName}' ile kullanıcı bulunamadı");
            }

            return ($" '{assignRole.Email}' sahip kullanıcı '{assignRole.RoleName}' eklendi");
        }

        public async Task<string> Create(RoleDto role)
        {
            /* bu rol var mı kontrol*/
            if (await _roleManager.RoleExistsAsync(role.RoleName))
            {
                await _redisService.PublishLogAsync("log_channel", new LogMessage
                {
                    ServiceName = "Auth / ROLE",
                    Level = LogDataMessage.LogLevel.Information,
                    Message = "Daha önce eklenmiş role eklenmeye çalışıldı",
                });
                return ($"'{role.RoleName}' bu role zaten var ");
            }
            /*rol ekleme*/
            var result = await _roleManager.CreateAsync(new IdentityRole(role.RoleName));
            if (!result.Succeeded)
            {
                await _redisService.PublishLogAsync("log_channel", new LogMessage
                {
                    ServiceName = "Auth / ROLE",
                    Level = LogDataMessage.LogLevel.Error,
                    Exception = result.Errors.ToString(),
                    Message = "Role create fail",
                });
                return $"{ result.Errors}";
            }

            return ($"{role.RoleName} eklendi");
        }
    }
}
