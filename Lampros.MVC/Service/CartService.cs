using Lampros.MVC.Models;
using Lampros.MVC.Service.IService;
using static Lampros.MVC.Utility.StaticTypes;

namespace Lampros.MVC.Service
{
    public class CartService : ICartService
    {
        private readonly IBaseService _baseService;
        public CartService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> ApplyCouponAsync(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.POST,
                Url = ShoppingCartApiBase + "/api/cart/ApplyCoupon",
                Data = cartDto
            });
        }

       

        public async Task<ResponseDto?> GetCartByUserIdAsync(string userId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.GET,
                Url = ShoppingCartApiBase + "/api/cart/GetCart/"+userId
            });
        }

       

        public async Task<ResponseDto?> RemoveFromCartAsync(int cartDetailsId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.POST,
                Url = ShoppingCartApiBase + "/api/cart/RemoveCart",
                Data = cartDetailsId
            });
        }

        public async Task<ResponseDto?> UpsertCartAsync(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.POST,
                Url = ShoppingCartApiBase + "/api/cart/CartUpsert",
                Data = cartDto
            });
        }
    }
}
