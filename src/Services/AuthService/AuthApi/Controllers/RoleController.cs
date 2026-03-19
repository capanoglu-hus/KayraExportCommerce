using AuthApi.Dtos;
using AuthApi.Models;
using AuthApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace AuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController(IRoleService service) : ControllerBase
    {
        

        [HttpPost("create")]
        public async Task<IActionResult> CreateRole( RoleDto role)
        {
            var result = await service.Create(role);
            return result is null ? BadRequest("Auth service role create fail ") : Ok(result);
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignRole(AssignRoleDto dto)
        {
            var result = await service.AssignRole(dto);
            return result is null ? BadRequest("Auth service role assign fail ") : Ok(result);
        }
    }
}
