using Lampros.MVC.Models;

namespace Lampros.MVC.Service.IService
{
    public interface IOrderService
    {
        Task<ResponseDto?> CreateOrder(CartDto cartDto);
        


    }
}
