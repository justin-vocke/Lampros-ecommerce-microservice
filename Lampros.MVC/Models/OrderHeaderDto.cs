

using static Lampros.MVC.Utility.StaticTypes;

namespace Lampros.MVC.Models
{
    public class OrderHeaderDto
    {
        public int OrderHeaderId { get; set; }
        public string? UserId { get; set; }
        public string? CouponCode { get; set; }

        public double Discount { get; set; }

        public double OrderTotal { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public DateTime OrderTime { get; set; }
        public OrderStatus? Status { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? StripeSessionId { get; set; }
        public IEnumerable<OrderDetailsDto> OrderDetails { get; set; }

        
    }
}
