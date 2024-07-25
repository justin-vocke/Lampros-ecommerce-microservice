using Lampros.MVC.Models;
using Lampros.MVC.Service.IService;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<ResponseDto?> CreateStripeSession(StripeRequestDto stripeRequesttDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.POST,
                Url = OrderApiBase + "/api/order/CreateStripeSession",
                Data = stripeRequesttDto
            });
        }

        public async Task<ResponseDto?> GetAllOrders(string? userId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.GET,
                Url = OrderApiBase + "/api/order/GetOrders?userId=" + userId
            });
        }

        public async Task<ResponseDto?> GetOrder(int orderId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.GET,
                Url = OrderApiBase + "/api/order/GetOrder/" + orderId                
            });
        }

        public async Task<ResponseDto?> UpdateOrderStatus(int orderId, [FromBody] string newStatus)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.POST,
                Url = OrderApiBase + "/api/order/UpdateOrderStatus/" + orderId,
                Data = newStatus
            });
        }

        public async Task<ResponseDto?> ValidateStripeSession(int orderHeaderId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.POST,
                Url = OrderApiBase + "/api/order/ValidateStripeSession",
                Data = orderHeaderId
            });
        }
    }
}
