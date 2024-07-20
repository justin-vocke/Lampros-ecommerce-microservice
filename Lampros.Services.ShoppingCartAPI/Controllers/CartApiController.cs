using AutoMapper;
using Lampros.Services.ShoppingCartAPI.Data;
using Lampros.Services.ShoppingCartAPI.Models;
using Lampros.Services.ShoppingCartAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Lampros.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private ResponseDto _response;
        private IMapper _mapper;
        private readonly ShoppingCartDbContext _context;

        public CartAPIController(IMapper mapper, ShoppingCartDbContext context)
        {
            this._response = new ResponseDto();
            _mapper = mapper;
            _context = context;
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

        [HttpPost("CartUpsert")]
        public async Task<ResponseDto> RemoveCart(CartDto cartDto)
        {
            //Tutorial has any changes to cart (add item, change item count, etc.) make a call to Db. 
            //This is excessive and expensive and I'll be looking to aggregate this perhaps with an event or 
            //caching or both then make a call to DB
            try
            {
                var cartHeaderFromId = await _context.CartHeader.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == cartDto.CartHeader.UserId);
                if (cartHeaderFromId is null)
                {
                    //need to create new cart header and cart details
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                    _context.CartHeader.Add(cartHeader);
                    await _context.SaveChangesAsync();
                    cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    _context.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                    await _context.SaveChangesAsync();
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

    }
}
