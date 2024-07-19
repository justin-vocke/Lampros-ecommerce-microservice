﻿using AutoMapper;
using Lampros.Services.ProductAPI.Data;
using Lampros.Services.ProductAPI.Models;
using Lampros.Services.ProductAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lampros.Services.ProductAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly ProductDbContext _context;
        private ResponseDto _responseDto;
        private readonly IMapper _mapper;

        public ProductController(ProductDbContext context, IMapper mapper)
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
                var ProductDtoList = _context.Products.ToList();
                _responseDto.Result = _mapper.Map<IEnumerable<ProductDto>>(ProductDtoList);

            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;

            }
            return _responseDto;
        }

        [HttpGet]
        [Route("{productId:int}")]
        public ResponseDto Get(int productId)
        {
            try
            {
                var product = _context.Products.First(x => x.ProductId == productId);
                _responseDto.Result = _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;

            }
            return _responseDto;
        }

        //There could be multiple products with the same name if no constraints are enforced. This is just for
        //demo purposes.
        [HttpGet]
        [Route("GetByCode/{name}")]
        public ResponseDto GetByCode(string name)
        {
            try
            {
                var product = _context.Products.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
                if (product is null)
                {
                    _responseDto.IsSuccess = false;
                }
                _responseDto.Result = _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;

            }
            return _responseDto;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<ResponseDto> Post([FromBody] ProductDto ProductDto)
        {
            try
            {
                var product = _mapper.Map<Product>(ProductDto);
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();
                _responseDto.Result = _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;

            }
            return _responseDto;
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public async Task<ResponseDto> Put([FromBody] ProductDto ProductDto)
        {
            try
            {
                var product = _mapper.Map<Product>(ProductDto);
                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                _responseDto.Result = _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;

            }
            return _responseDto;
        }

        [HttpDelete]
        [Route("{productId:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ResponseDto> Delete(int productId)
        {
            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == productId);
                _context.Products.Remove(product);
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
