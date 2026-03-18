using AuthApi.Dtos;
using AuthApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService service) : ControllerBase
    {
        
        [HttpPost("register")]
        public async Task<IActionResult> Register( RegisterDto register)
        {
            var address = GetIpAddress();
            var result = await service.Register(register, address);
            return result ? Ok("Kullanıcı ekleme başarılı"): BadRequest("Kullanıcı eklenirken bir hata oluştu");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login( LoginDto login)
        {
            var address = GetIpAddress();
            var result = await service.Login(login, address);
            return Ok(result);

        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken( TokenRequestDto request)
        {
            var address = GetIpAddress();
            var result = await service.RefreshToken(request, address);
            return Ok(result);
        }
        
        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke( TokenRequestDto request )
        {
            var address = GetIpAddress();
            var result = await service.Revoke(request, address);
            return result ? Ok("oturum kapatma başarılı") : BadRequest("oturum kapatılırken bir hata oluştu");
        }
        /*atılan requestten ip adresi alma*/
        private string GetIpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                return Request.Headers["X-Forwarded-For"].ToString();
            }
            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknowm";
        }
    }
}
