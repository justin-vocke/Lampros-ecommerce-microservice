using Lampros.MVC.Models.Dto;
using Lampros.MVC.Service.IService;
using Microsoft.AspNetCore.Mvc;

namespace Lampros.MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDto loginRequest = new LoginRequestDto();
        }
    }
}
