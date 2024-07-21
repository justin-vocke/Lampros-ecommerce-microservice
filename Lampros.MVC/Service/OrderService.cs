using Lampros.MVC.Models;
using Lampros.MVC.Service.IService;
using static Lampros.MVC.Utility.StaticTypes;

namespace Lampros.MVC.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBaseService _baseService;
        public OrderService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        
        public async Task<ResponseDto?> CreateOrder(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.POST,
                Url = OrderApiBase + "/api/order/CreateOrder",
                Data = cartDto
            });
        }

        
    }
}
