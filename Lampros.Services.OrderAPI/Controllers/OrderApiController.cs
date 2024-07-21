using AutoMapper;
using Lampros.Services.OrderAPI.Data;
using Lampros.Services.OrderAPI.Models;
using Lampros.Services.OrderAPI.Models.Dto;
using Lampros.Services.OrderAPI.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Lampros.Services.OrderAPI.Utility.StaticTypes;

namespace Lampros.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderApiController : ControllerBase
    {
        private ResponseDto _responseDto;
        private IMapper _mapper;
        private readonly OrderDbContext _dbContext;
        private IProductService _productService;
        public OrderApiController(IMapper mapper, OrderDbContext dbContext, IProductService productService)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _productService = productService;
            _responseDto = new ResponseDto();
        }

        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ResponseDto> CreateOrder([FromBody] CartDto cartDto)
        {
            try
            {
                OrderHeaderDto orderHeaderDto = _mapper.Map<OrderHeaderDto>(cartDto.CartHeader);
                orderHeaderDto.OrderTime = DateTime.Now;
                orderHeaderDto.Status = OrderStatus.Pending;
                orderHeaderDto.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDto>>(cartDto.CartDetails);

                OrderHeader orderCreated = _dbContext.Add(_mapper.Map<OrderHeader>(orderHeaderDto)).Entity;
                await _dbContext.SaveChangesAsync();

                orderHeaderDto.OrderHeaderId = orderCreated.OrderHeaderId;
                _responseDto.Result = orderHeaderDto;
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
