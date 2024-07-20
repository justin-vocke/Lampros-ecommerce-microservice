using Lampros.Services.ShoppingCartAPI.Models.Dto;

namespace Lampros.Services.ShoppingCartAPI.Service.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProductsAsync();
    }
}
