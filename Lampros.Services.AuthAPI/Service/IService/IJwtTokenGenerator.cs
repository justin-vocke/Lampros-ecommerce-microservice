using Lampros.Services.AuthAPI.Models;

namespace Lampros.Services.AuthAPI.Service.IService
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(ApplicationUser applicationUser); 

    }
}
