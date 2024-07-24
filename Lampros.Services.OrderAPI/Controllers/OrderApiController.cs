using AutoMapper;
using Lampros.MessageBus;
using Lampros.Services.OrderAPI.Data;
using Lampros.Services.OrderAPI.Models;
using Lampros.Services.OrderAPI.Models.Dto;
using Lampros.Services.OrderAPI.Service.IService;
using Lampros.Services.OrderAPI.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using Stripe.Climate;
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
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        public OrderApiController(IMapper mapper, OrderDbContext dbContext, IProductService productService, IMessageBus messageBus, IConfiguration configuration)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _productService = productService;
            _responseDto = new ResponseDto();
            _messageBus = messageBus;
            _configuration = configuration;
        }

        [HttpGet("GetOrders")]
        [Authorize]
        public async Task<ResponseDto> GetOrders(string? userId = "")
        {
            try
            {
                IEnumerable<OrderHeader> orderHeaderList;
                if (User.IsInRole(StaticTypes.RoleAdmin))
                {
                    orderHeaderList = _dbContext.OrderHeader.Include(u => u.OrderDetails).OrderByDescending(x => x.OrderHeaderId).ToList();
                }
                else
                {
                    orderHeaderList = _dbContext.OrderHeader.Include(u => u.OrderDetails).
                        Where(x => x.UserId == userId).OrderByDescending(x => x.OrderHeaderId).ToList();
                }
                _responseDto.Result = _mapper.Map<IEnumerable<OrderHeaderDto>>(orderHeaderList);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        [HttpGet("GetOrder/{orderId:int}")]
        [Authorize]
        public async Task<ResponseDto> GetOrder(int orderId)
        {
            try
            {
                OrderHeader orderHeader = _dbContext.OrderHeader.Include(u => u.OrderDetails).First(x => x.OrderHeaderId == orderId);
                _responseDto.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
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

        [Authorize]
        [HttpPost("CreateStripeSession")]
        public async Task<ResponseDto> CreateStripeSession([FromBody] StripeRequestDto stripeRequestDto)
        {
            try
            {

                
                var options = new SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDto.ApprovedUrl,
                    CancelUrl = stripeRequestDto.CancelUrl,
                    LineItems = new List<SessionLineItemOptions>(),
                    
                    Mode = "payment",
                };

                var discountsObj = new List<SessionDiscountOptions>()
                {
                    new SessionDiscountOptions
                    {
                        Coupon = stripeRequestDto.OrderHeader.CouponCode
                    }
                };

                foreach (var item in stripeRequestDto.OrderHeader.OrderDetails)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }
                if(stripeRequestDto.OrderHeader.Discount > 0)
                {
                    options.Discounts = discountsObj;
                }
                var service = new SessionService();
                Session session = service.Create(options);
                stripeRequestDto.StripeSessionUrl = session.Url;
                OrderHeader orderHeader = _dbContext.OrderHeader.First(x => x.OrderHeaderId == stripeRequestDto.OrderHeader.OrderHeaderId);
                orderHeader.StripeSessionId = session.Id;
                await _dbContext.SaveChangesAsync();
                _responseDto.Result = stripeRequestDto;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        [Authorize]
        [HttpPost("ValidateStripeSession")]
        public async Task<ResponseDto> ValidateStripeSession([FromBody] int orderHeaderId)
        {
            try
            {
                OrderHeader orderHeader = _dbContext.OrderHeader.First(x => x.OrderHeaderId == orderHeaderId);

                var service = new SessionService();
                Session session = service.Get(orderHeader.StripeSessionId);

                var paymentIntentService = new PaymentIntentService();
                PaymentIntent paymentIntent = paymentIntentService.Get(session.PaymentIntentId);

                if(paymentIntent.Status == "succeeded")
                {
                    //then payment was successful
                    orderHeader.PaymentIntentId = paymentIntent.Id;
                    orderHeader.Status = OrderStatus.Approved;
                    await _dbContext.SaveChangesAsync();
                    RewardDto rewardDto = new()
                    {
                        OrderId = orderHeader.OrderHeaderId,
                        RewardActivity = Convert.ToInt32(orderHeader.OrderTotal),
                        UserId = orderHeader.UserId
                    };

                    string topicName = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
                    await _messageBus.PublishMessage(rewardDto, topicName);
                    _responseDto.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
                }
                
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        [Authorize]
        [HttpPost("UpdateOrderStatus/{orderId:int}")]
        public async Task<ResponseDto> UpdateOrderStatus(int orderId, [FromBody] string newStatus)
        {
            try
            {
                StaticTypes.OrderStatus newStatusEnumValue;
                OrderHeader orderHeader = _dbContext.OrderHeader.First(x => x.OrderHeaderId == orderId);
                if(orderHeader is not null)
                {
                    if(newStatus == StaticTypes.OrderStatus.Cancelled.ToString())
                    {
                        //we will give refund
                        var options = new RefundCreateOptions
                        {
                            Reason = RefundReasons.RequestedByCustomer,
                            PaymentIntent = orderHeader.PaymentIntentId
                        };

                        var refundService = new RefundService();
                        Refund refund = await refundService.CreateAsync(options);
                        
                    }
                    Enum.TryParse(newStatus, out newStatusEnumValue);
                    orderHeader.Status = newStatusEnumValue;
                    await _dbContext.SaveChangesAsync();
                    
                }
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
            }
            return _responseDto;
        }

    }
}
