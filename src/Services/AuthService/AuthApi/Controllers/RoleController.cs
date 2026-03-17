using AuthApi.Dtos;
using AuthApi.Models;
using AuthApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace AuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IRedisService _redisService;


        public RoleController(IRedisService redisService, RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _redisService = redisService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateRole([FromBody] RoleDto role)
        {
            /* bu rol var mı kontrol*/
            if(await _roleManager.RoleExistsAsync(role.RoleName))
            {
                return BadRequest($"'{role.RoleName}' bu role zaten var ");
            }
            /*rol ekleme*/
            var result = await _roleManager.CreateAsync(new IdentityRole(role.RoleName));
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            
            
            return Ok($"''{role.RoleName} eklendi");
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
        {
            /*email ile role ataması yapar*/
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if(user == null)
            {
                return NotFound($" Bu '{dto.Email}' ile kullanıcı bulunamadı");
            }
            var result = await _userManager.AddToRoleAsync(user, dto.RoleName);
            if (result == null)
            {
                return NotFound($" Bu '{dto.RoleName}' ile kullanıcı bulunamadı");
            }

            

            return Ok($" '{dto.Email}' sahip kullanıcı '{dto.RoleName}' eklendi");
        }
    }
}
