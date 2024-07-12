using AutoMapper;
using Lampros.Services.CouponAPI.Models;
using Lampros.Services.CouponAPI.Models.Dto;

namespace Lampros.Services.CouponAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mapperConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CouponDto, Coupon>();
                config.CreateMap<Coupon, CouponDto>();

            });
            return mapperConfig;
        }
    }
}
