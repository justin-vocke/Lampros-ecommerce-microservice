namespace Lampros.Services.CouponAPI.Models.Dto
{
    public record CouponDto(int CouponId, string CouponCode, double DiscountAmount, int? MinAmount);
    
}
