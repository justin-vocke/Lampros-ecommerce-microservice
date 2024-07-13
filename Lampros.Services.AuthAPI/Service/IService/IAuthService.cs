using Lampros.Services.AuthAPI.Models.Dto;

namespace Lampros.Services.AuthAPI.Service.IService
{
    public interface IAuthService
    {
        Task<string> Register(RegistrationRequestDto registrationRequestDto);

        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
    }
}
