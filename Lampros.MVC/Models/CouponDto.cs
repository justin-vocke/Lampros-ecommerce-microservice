namespace Lampros.MVC.Models
{
    public record CouponDto(int CouponId, string CouponCode, double DiscountAmount, int? MinAmount);
    
}
