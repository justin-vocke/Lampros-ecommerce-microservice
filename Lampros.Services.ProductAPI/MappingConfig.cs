using AutoMapper;
using Lampros.Services.ProductAPI.Models;
using Lampros.Services.ProductAPI.Models.Dto;

namespace Lampros.Services.ProductAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mapperConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<ProductDto, Product>();
                config.CreateMap<Product, ProductDto>();

            });
            return mapperConfig;
        }
    }
}
