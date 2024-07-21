using Lampros.Services.EmailAPI.Models.Dto;

namespace Lampros.Services.EmailAPI.Services
{
    public interface IEmailService
    {
        Task EmailCartAndLog(CartDto cartDto);
    }
}
