using Lampros.MVC.Models;
using Lampros.MVC.Models.Dto;
using Lampros.MVC.Service.IService;
using static Lampros.MVC.Utility.StaticTypes;

namespace Lampros.MVC.Service
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;
        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDto?> AssignRoleAsync(RegistrationRequestDto registrationRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.POST,
                Url = AuthApiBase + "/api/auth/assignrole",
                Data = registrationRequestDto
            });
        }

        public async Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequest)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.POST,
                Url = AuthApiBase + "/api/auth/login",
                Data = loginRequest
            });
        }

        public async Task<ResponseDto?> RegisterAsync(RegistrationRequestDto registrationRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.POST,
                Url = AuthApiBase + "/api/auth/register",
                Data = registrationRequestDto
            });
        }
    }
}
