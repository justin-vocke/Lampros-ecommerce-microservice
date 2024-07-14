using Lampros.MVC.Models;
using Lampros.MVC.Service.IService;
using Lampros.MVC.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

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
            return View(loginRequest);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            ResponseDto responseDto = await _authService.LoginAsync(loginRequestDto);
           
            if (responseDto is not null && responseDto.IsSuccess)
            {
                LoginResponseDto loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(responseDto.Result));
                TempData["success"] = "Registration Successful";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("CustomerError", responseDto.Message);
                return View(loginRequestDto);
            }
            
        }
        [HttpGet]
        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text = StaticTypes.RoleAdmin, Value = StaticTypes.RoleAdmin},
                new SelectListItem {Text = StaticTypes.RoleCustomer,Value = StaticTypes.RoleCustomer},
            };

            ViewBag.RoleList = roleList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto registrationRequestDto)
        {
            ResponseDto responseDto = await _authService.RegisterAsync(registrationRequestDto);
            ResponseDto assignRole;
            if(responseDto is not null && responseDto.IsSuccess)
            {
                if(string.IsNullOrEmpty(registrationRequestDto.RoleName))
                {
                    registrationRequestDto.RoleName = StaticTypes.RoleCustomer;
                }
                
                    assignRole = await _authService.AssignRoleAsync(registrationRequestDto);
                    if(assignRole is not null && assignRole.IsSuccess)
                    {
                        TempData["success"] = "Registration Successful";
                        return RedirectToAction(nameof(Login));
                    }
                
            }
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text = StaticTypes.RoleAdmin, Value = StaticTypes.RoleAdmin},
                new SelectListItem {Text = StaticTypes.RoleCustomer,Value = StaticTypes.RoleCustomer},
            };

            ViewBag.RoleList = roleList;
            return View(registrationRequestDto);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            return View();
        }
    }
}
