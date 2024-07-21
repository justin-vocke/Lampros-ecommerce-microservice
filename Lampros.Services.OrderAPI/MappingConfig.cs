using AutoMapper;
using Lampros.Services.OrderAPI.Models;
using Lampros.Services.OrderAPI.Models.Dto;

namespace Lampros.Services.OrderAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mapperConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<OrderHeaderDto, CartHeaderDto>()
                .ForMember(dest => dest.CartTotal, u => u.MapFrom(src => src.OrderTotal)).ReverseMap();

                config.CreateMap<CartDetailsDto, OrderDetailsDto>()
                .ForMember(dest => dest.ProductName, u => u.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.Price, u => u.MapFrom(src => src.Product.Price));

                config.CreateMap<OrderDetailsDto, CartDetailsDto>();
                config.CreateMap<OrderDetails, OrderDetailsDto>().ReverseMap();
                config.CreateMap<OrderHeader, OrderHeaderDto>().ReverseMap();
            });
            return mapperConfig;
        }
    }
}
