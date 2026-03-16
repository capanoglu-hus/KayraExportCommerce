using AuthApi.Data;
using AuthApi.Dtos;
using AuthApi.Models;
using AuthApi.Services;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly UserDbContext _context;
        private readonly ITokenService _tokenService;

        public AuthController(UserManager<User> userManager, UserDbContext context, ITokenService tokenService)
        {
            _userManager = userManager;
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto register)
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
                return BadRequest(result.Errors);
            }
            return Ok(new { Message = " Kullanıcı oluşturuldu" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.Email == login.Email);
            /* ilgili kullanıcıyı email üzerinden buluyor */
            if(user == null || !await _userManager.CheckPasswordAsync(user, login.Password))
            {
                /* kullanıcı olup olmadığına ve pass kontrolu yapıyor*/
                return Unauthorized(new { Message = "email veya şifre yanlış" });

            }
            var roles = await _userManager.GetRolesAsync(user);
            // kullnaıcının rolunu alıyor
            var accessToken = _tokenService.CreateAccessToken(user, roles);
            // yeni jwt token  
            var refreshToken = _tokenService.CreateRefreshToken(GetIpAddress());


            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);
            return Ok(new TokenResponseDto { AccessToken = accessToken, RefreshToken = refreshToken.Token });
           
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto request)
        {
            var refreshToken = request.RefreshToken;
            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken));
            /* o tokenı kullanan kullanıcıyı buluyor*/
            if(user == null)
            {
                return Unauthorized(new { Message = " geçersiz refresh token" });
            }
            var existingToken = user.RefreshTokens.Single(t => t.Token == refreshToken);
            if (!existingToken.isActive)
            {
                return Unauthorized(new { Message = " refresh token aktif değil" });
            }
            /*refresh tokenı iptal ediyor*/
            existingToken.Revoked = DateTime.UtcNow;
            existingToken.RevokedByIp = GetIpAddress();
            // yenisini oluşturup veritabanına ekliyor
            var newRefreshToken = _tokenService.CreateRefreshToken(GetIpAddress());
            existingToken.ReplacedByToken = newRefreshToken.Token;
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);
            //yeni access token 
            var roles = await _userManager.GetRolesAsync(user);
            var newAccessToken = _tokenService.CreateAccessToken(user, roles);
            return Ok(new TokenResponseDto { AccessToken = newAccessToken, RefreshToken = newRefreshToken.Token });
        }
        
        
        
        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke([FromBody] TokenRequestDto request)
        {
            /*oturum kapatma*/
            var token = request.RefreshToken;
            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));
            if (user == null)
            {
                return NotFound();
            }
            var existing = user.RefreshTokens.Single(t => t.Token == token);
            if (!existing.isActive)
            {
                return BadRequest(new { Message = "zaten iptal " });
            }
            existing.Revoked = DateTime.UtcNow;
            existing.RevokedByIp = GetIpAddress();
            await _userManager.UpdateAsync(user);
            return Ok(new { Message = "iptal edildi" });
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
