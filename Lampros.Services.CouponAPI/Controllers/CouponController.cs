using AutoMapper;
using Lampros.Services.CouponAPI.Data;
using Lampros.Services.CouponAPI.Models;
using Lampros.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lampros.Services.CouponAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly CouponDbContext _context;
        private ResponseDto _responseDto;
        private readonly IMapper _mapper;

        public CouponController(CouponDbContext context, IMapper mapper)
        {
            _context = context;
            _responseDto = new ResponseDto();
            _mapper = mapper;
        }

        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                var couponDtoList = _context.Coupons.ToList();
                _responseDto.Result = _mapper.Map<IEnumerable<CouponDto>>(couponDtoList);

            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
                
            }
            return _responseDto;
        }

        [HttpGet]
        [Route("{couponId:int}")]
        public ResponseDto Get(int couponId)
        {
            try
            {
                var coupon =   _context.Coupons.First(x => x.CouponId == couponId);
                _responseDto.Result = _mapper.Map<CouponDto>(coupon);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
                
            }
            return _responseDto;
        }

        [HttpGet]
        [Route("GetByCode/{couponcode}")]
        public ResponseDto GetByCode(string couponcode)
        {
            try
            {
                var coupon = _context.Coupons.FirstOrDefault(x => x.CouponCode.ToLower() == couponcode.ToLower());
                if(coupon is null)
                {
                    _responseDto.IsSuccess = false;
                }
                _responseDto.Result = _mapper.Map<CouponDto>(coupon);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;

            }
            return _responseDto;
        }

        [HttpPost]
        public async Task<ResponseDto> Post([FromBody] CouponDto couponDto)
        {
            try
            {
                var coupon = _mapper.Map<Coupon>(couponDto);
                await _context.Coupons.AddAsync(coupon);
                await _context.SaveChangesAsync();
                _responseDto.Result = _mapper.Map<CouponDto>(coupon);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;

            }
            return _responseDto;
        }

        [HttpPut]
        public async Task<ResponseDto> Put([FromBody] CouponDto couponDto)
        {
            try
            {
                var coupon = _mapper.Map<Coupon>(couponDto);
                 _context.Coupons.Update(coupon);
                await _context.SaveChangesAsync();

                _responseDto.Result = _mapper.Map<CouponDto>(coupon);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;

            }
            return _responseDto;
        }

        [HttpDelete]
        [Route("{couponId:int}")]
        public async Task<ResponseDto> Delete(int couponId)
        {
            try
            {
                var coupon = await _context.Coupons.FirstOrDefaultAsync(x => x.CouponId == couponId);
                _context.Coupons.Remove(coupon);
                await _context.SaveChangesAsync();

               
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;

            }
            return _responseDto;
        }


    }
}
