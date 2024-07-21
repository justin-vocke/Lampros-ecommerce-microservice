namespace Lampros.Services.OrderAPI.Utility
{
    public class StaticTypes
    {
        public enum OrderStatus
        {
            Pending,
            Approved,
            ReadyForPickup,
            Completed,
            Refunded,
            Cancelled
        }

        public const string RoleAdmin = "ADMIN";
        public const string RoleCustomer = "CUSTOMER";
    }
}
