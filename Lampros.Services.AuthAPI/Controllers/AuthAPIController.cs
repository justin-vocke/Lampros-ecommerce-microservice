using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lampros.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register()
        {
            return Ok();
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login()
        {
            return Ok();
        }
    }
}
