using AutoMapper;
using Lampros.MessageBus;
using Lampros.Services.ShoppingCartAPI.Data;
using Lampros.Services.ShoppingCartAPI.Models;
using Lampros.Services.ShoppingCartAPI.Models.Dto;
using Lampros.Services.ShoppingCartAPI.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Lampros.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        //Tutorial has all business logic and data access in controller. Super bad practice and will refactor to make 
        //controllers thin
        private ResponseDto _response;
        private readonly IMapper _mapper;
        private readonly ShoppingCartDbContext _context;
        private readonly IProductService _productService;
        private readonly IConfiguration _configuration;
        private readonly IMessageBus _messageBus;
        private readonly ICouponService _couponService;
        

        public CartAPIController(IMapper mapper, ShoppingCartDbContext context, IProductService productService,
            ICouponService couponService, IMessageBus messageBus, IConfiguration configuration)
        {
            this._response = new ResponseDto();
            _mapper = mapper;
            _context = context;
            _productService = productService;
            _couponService = couponService;
            _messageBus = messageBus;
            _configuration = configuration;
        }
        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
        {
            try
            {
                CartDto cartDto = new()
                {
                    CartHeader = _mapper.Map<CartHeaderDto>(_context.CartHeader.First(x => x.UserId == userId)),

                };
                cartDto.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(_context.CartDetails
                    .Where(x => x.CartHeaderId == cartDto.CartHeader.CartHeaderId));

                IEnumerable<ProductDto> productDtos = await _productService.GetProductsAsync();
                foreach(var item in  cartDto.CartDetails)
                {
                    item.Product = productDtos.FirstOrDefault(x => x.ProductId == item.ProductId);
                    cartDto.CartHeader.CartTotal += (item.Count * item.Product.Price);
                }
                //apply coupon if any
                if (!string.IsNullOrEmpty(cartDto.CartHeader.CouponCode))
                {
                    CouponDto couponDto = await _couponService.GetCouponAsync(cartDto.CartHeader.CouponCode);
                    if(couponDto is not null && cartDto.CartHeader.CartTotal > couponDto.MinAmount)
                    {
                        cartDto.CartHeader.CartTotal -= couponDto.DiscountAmount;
                        cartDto.CartHeader.Discount = couponDto.DiscountAmount;
                    }
                }
                _response.Result = cartDto;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartFromDb = await _context.CartHeader.FirstAsync(x => x.UserId == cartDto.CartHeader.UserId);
                cartFromDb.CouponCode = cartDto.CartHeader.CouponCode;
                _context.CartHeader.Update(cartFromDb);
                await _context.SaveChangesAsync();
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }

        [HttpPost("RemoveCoupon")]
        public async Task<object> RemoveCoupon(CartDto cartDto)
        {
            try
            {
                var cartFromDb = await _context.CartHeader.FirstAsync(x => x.UserId == cartDto.CartHeader.UserId);
                cartFromDb.CouponCode = "";
                _context.CartHeader.Update(cartFromDb);
                await _context.SaveChangesAsync();
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }

        [HttpPost("EmailCartRequest")]
        public async Task<object> EmailCartRequest([FromBody] CartDto cartDto)
        {
            try
            {
                await _messageBus.PublishMessage(cartDto, _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue"));
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }


        [HttpPost("CartUpsert")]
        public async Task<ResponseDto> CartUpsert(CartDto cartDto)
        {
            //Tutorial has any changes to cart (add item, change item count, etc.) make a call to Db. 
            //This is excessive and expensive and I'll be looking to aggregate this perhaps with an event or 
            //caching or both then make a call to DB
            try
            {
                var cartHeaderFromId = await _context.CartHeader.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == cartDto.CartHeader.UserId);
                if(cartHeaderFromId is null)
                {
                    //need to create new cart header and cart details
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                    _context.CartHeader.Add(cartHeader);
                    await _context.SaveChangesAsync();
                    cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    _context.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                    await _context.SaveChangesAsync();
                }
                else
                {
                    //check if details has some product
                    var cartDetailsFromDb = await _context.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                        x => x.ProductId == cartDto.CartDetails.First().ProductId &&
                        x.CartHeaderId == cartHeaderFromId.CartHeaderId);
                    if(cartDetailsFromDb is null)
                    {
                        //create new cartDetails
                        cartDto.CartDetails.First().CartHeaderId = cartHeaderFromId.CartHeaderId;
                        _context.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        //update count in cart details
                        cartDto.CartDetails.First().Count += cartDetailsFromDb.Count; 
                        cartDto.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId; 
                        cartDto.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;

                        _context.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _context.SaveChangesAsync();
                    }
                }
                _response.Result = cartDto;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }

        [HttpPost("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody]int cartDetailsId)
        {
            //Tutorial has any changes to cart (add item, change item count, etc.) make a call to Db. 
            //This is excessive and expensive and I'll be looking to aggregate this perhaps with an event or 
            //caching or both then make a call to DB
            try
            {
                CartDetails cartDetails = _context.CartDetails.First(x => x.CartDetailsId == cartDetailsId);

                int totalCountOfCartItems = _context.CartDetails.Where(x => x.CartHeaderId == cartDetails.CartHeaderId).Count();
                _context.CartDetails.Remove(cartDetails);
                if(totalCountOfCartItems == 1)
                {
                    
                    //need to create new cart header and cart details
                    CartHeader cartHeaderToRemove = await _context.CartHeader
                        .FirstOrDefaultAsync(x => x.CartHeaderId == cartDetails.CartHeaderId);
                    _context.CartHeader.Remove(cartHeaderToRemove);
                    
                }
                await _context.SaveChangesAsync();
               
                
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }

    }
}
