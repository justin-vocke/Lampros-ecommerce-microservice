using Lampros.Services.ShoppingCartAPI.Models.Dto;

namespace Lampros.Services.ShoppingCartAPI.Service.IService
{
    public interface ICouponService
    {
        Task<CouponDto> GetCouponAsync(string couponCode);
    }
}
