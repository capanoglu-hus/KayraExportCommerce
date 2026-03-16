using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        
        [HttpGet("public")]
        public IActionResult Public()
        {
            return Ok("public");
        }

        [Authorize]
        [HttpGet("protected")]
        public IActionResult Protected ()
        {
            return Ok("protected");
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public IActionResult Admin()
        {
            return Ok("Admin");
        }
    }
}
