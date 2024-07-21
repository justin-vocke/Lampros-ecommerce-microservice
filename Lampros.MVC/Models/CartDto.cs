using Lampros.MVC.Models.Dto;

namespace Lampros.MVC.Models
{
    public class CartDto
    {
        public CartHeaderDto CartHeader { get; set; }
        public IEnumerable<CartDetailsDto?> CartDetails { get; set; }
    }
}