using Lampros.Services.EmailAPI.Message;
using Lampros.Services.EmailAPI.Models.Dto;

namespace Lampros.Services.EmailAPI.Services
{
    public interface IEmailService
    {
        Task EmailCartAndLog(CartDto cartDto);
        Task RegisterUserEmailAndLog(string email);
        Task LogOrderPlaced(RewardsMessage rewardsMessage);
    }
}
