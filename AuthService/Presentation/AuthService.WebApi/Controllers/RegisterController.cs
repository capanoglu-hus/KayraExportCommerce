using AuthService.Application.CQRSDesign.Commands;
using AuthService.Application.CQRSDesign.Handler;
using AuthService.Application.Dtos;
using AuthService.Application.Services.Concrate;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly CreateUserRegisterCommandHandler _createUserRegisterCommandHandler;
        private readonly AuthjwtService _service;

        public RegisterController(AuthjwtService service , CreateUserRegisterCommandHandler createUserRegisterCommandHandler)
        {
            _createUserRegisterCommandHandler = createUserRegisterCommandHandler;
            _service = service;
        }
        [HttpPost]
        public async Task<IActionResult> CreateUserRegister(CreateUserRegisterCommand command)
        {
            await _createUserRegisterCommandHandler.Handle(command);
            return Ok("Kullanıcı Eklendi");
        }

        [HttpPost("generateToken")]
        public async Task<IActionResult> GenerateToken(TokenClaimsDto token)
        {
            var result = await _service.GenerateToken(token);
            if (result.Data == null)
            {
                return BadRequest("Token oluşturulamadı.");
            }
            return Ok(result.Data.ToString());
        }
    }
}
