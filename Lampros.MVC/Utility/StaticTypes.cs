namespace Lampros.MVC.Utility
{
    public class StaticTypes
    {
        public static string CouponApiBase { get; set; }
        public static string AuthApiBase { get; set; }

        public const string RoleAdmin = "ADMIN";
        public const string RoleCustomer = "CUSTOMER";

        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }
    }
}
