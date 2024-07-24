using Lampros.MVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lampros.MVC.Service.IService
{
    public interface IOrderService
    {
        Task<ResponseDto?> CreateOrder(CartDto cartDto);
        Task<ResponseDto?> CreateStripeSession(StripeRequestDto stripeRequesttDto);
        Task<ResponseDto?> ValidateStripeSession(int orderHeaderId);       
        Task<ResponseDto?> GetAllOrders(string? userId);       
        Task<ResponseDto?> GetOrder(int orderId);       
        Task<ResponseDto?> UpdateOrderStatus(int orderId, [FromBody] string newStatus);       


    }
}
