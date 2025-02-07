﻿using Lampros.MVC.Models;
using Lampros.MVC.Service;
using Lampros.MVC.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Lampros.MVC.Controllers
{
    
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> ProductIndex()
        {
            List<ProductDto?> productDtos = new();
            ResponseDto? response = await _productService.GetAllProductsAsync();

            if(response is not null && response.IsSuccess)
            {
                productDtos = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(productDtos);
        }

        public async Task<IActionResult> ProductCreate()
        {

            return View();
        }

        [HttpPost]
		public async Task<IActionResult> ProductCreate(ProductDto productDto)
		{
            if (ModelState.IsValid)
            {
				
				ResponseDto? response = await _productService.CreateProductAsync(productDto);

				if (response is not null && response.IsSuccess)
				{
                    TempData["success"] = "Product Created Successfully";
                    return RedirectToAction(nameof(ProductIndex));
				}
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
			return View(productDto);
		}

        public async Task<IActionResult> ProductEdit(int productId)
        {
            ResponseDto? response = await _productService.GetProductByIdAsync(productId);

            if (response is not null && response.IsSuccess)
            {
                ProductDto productDto = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(productDto);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> ProductEdit(ProductDto productDto)
        {
            if (ModelState.IsValid)
            {

                ResponseDto? response = await _productService.UpdateProductAsync(productDto);

                if (response is not null && response.IsSuccess)
                {
                    TempData["success"] = "Product Edited Successfully";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(productDto);
        }

        public async Task<IActionResult> ProductDelete(int productId)
		{
			ResponseDto? response = await _productService.GetProductByIdAsync(productId);

			if (response is not null && response.IsSuccess)
			{
				ProductDto productDto = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(productDto);
			}
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
		}
        [HttpPost]
        public async Task<IActionResult> ProductDelete(ProductDto productDto)
        {
            ResponseDto? response = await _productService.DeleteProductAsync(productDto.ProductId);

            if (response is not null && response.IsSuccess)
            {
                TempData["success"] = "Product Deleted Successfully";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(productDto);
        }
    }
}
