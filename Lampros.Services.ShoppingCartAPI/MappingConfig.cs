using AutoMapper;
using Lampros.Services.ShoppingCartAPI.Models;
using Lampros.Services.ShoppingCartAPI.Models.Dto;

namespace Lampros.Services.ShoppingCartAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mapperConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CartDetails, CartDetailsDto>().ReverseMap();
                config.CreateMap<CartHeader, CartHeader>().ReverseMap();
            });
            return mapperConfig;
        }
    }
}
