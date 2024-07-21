

using Lampros.Services.OrderAPI.Models.Dto;

namespace Lampros.Services.OrderAPI.Service.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProductsAsync();
    }
}
