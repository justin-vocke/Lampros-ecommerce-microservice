using Lampros.MVC.Models;
using Lampros.MVC.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Lampros.MVC.Controllers
{
    
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        public async Task<IActionResult> CouponIndex()
        {
            List<CouponDto?> couponDtos = new();
            ResponseDto? response = await _couponService.GetAllCouponsAsync();

            if(response is not null && response.IsSuccess)
            {
                couponDtos = JsonConvert.DeserializeObject<List<CouponDto>>(Convert.ToString(response.Result));
            }
            return View(couponDtos);
        }

        public async Task<IActionResult> CouponCreate()
        {

            return View();
        }

        [HttpPost]
		public async Task<IActionResult> CouponCreate(CouponDto couponDto)
		{
            if (ModelState.IsValid)
            {
				
				ResponseDto? response = await _couponService.CreateCouponAsync(couponDto);

				if (response is not null && response.IsSuccess)
				{
                    return RedirectToAction(nameof(CouponIndex));
				}
			}
			return View(couponDto);
		}

		public async Task<IActionResult> CouponDelete(int couponId)
		{
			ResponseDto? response = await _couponService.GetCouponByIdAsync(couponId);

			if (response is not null && response.IsSuccess)
			{
				CouponDto couponDto = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response.Result));
                return View(couponDto);
			}
			return NotFound();
		}
        [HttpPost]
        public async Task<IActionResult> CouponDelete(CouponDto couponDto)
        {
            ResponseDto? response = await _couponService.DeleteCouponAsync(couponDto.CouponId);

            if (response is not null && response.IsSuccess)
            {
                return RedirectToAction(nameof(CouponIndex));
            }
            return View(couponDto);
        }
    }
}
