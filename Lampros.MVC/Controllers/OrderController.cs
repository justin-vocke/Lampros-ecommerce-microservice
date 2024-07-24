using Lampros.MVC.Models;
using Lampros.MVC.Models.Dto;
using Lampros.MVC.Service.IService;
using Lampros.MVC.Utility;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Lampros.MVC.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public IActionResult OrderIndex()
        {
            return View();
        }

        public async Task<IActionResult> OrderDetail(int orderId)
        {
            OrderHeaderDto orderHeaderDto = new OrderHeaderDto();
            string userId = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;

            var responseDto = await _orderService.GetOrder(orderId);

            if (responseDto is not null && responseDto.IsSuccess)
            {
                orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(responseDto.Result));
            }
           if(!User.IsInRole(StaticTypes.RoleAdmin) && userId != orderHeaderDto.UserId)
            {
                return NotFound();
            }
            return View(orderHeaderDto);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(string orderStatus)
        {
            IEnumerable<OrderHeaderDto> orderList;
            string userId = "";
            if(!User.IsInRole(StaticTypes.RoleAdmin))
            {
                userId = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            }
            ResponseDto responseDto = await _orderService.GetAllOrders(userId);

            if(responseDto is not null && responseDto.IsSuccess)
            {
                orderList = JsonConvert.DeserializeObject<IEnumerable<OrderHeaderDto>>(Convert.ToString(responseDto.Result));
                switch(orderStatus)
                {
                    case "approved":
                        orderList = orderList.Where(x => x.Status == StaticTypes.OrderStatus.Approved).ToList();
                        break;
                    case "readyforpickup":
                        orderList = orderList.Where(x => x.Status == StaticTypes.OrderStatus.ReadyForPickup).ToList();
                        break;
                    case "cancelled":
                        orderList = orderList.Where(x => x.Status == StaticTypes.OrderStatus.Cancelled).ToList();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                orderList = new List<OrderHeaderDto>();
            }
            return Json(new {data = orderList });
        }

        [HttpPost("OrderReadyForPickup")]
        public async Task<IActionResult> OrderReadyForPickup(int orderId)
        {
            var responseDto = await _orderService.UpdateOrderStatus(orderId, StaticTypes.OrderStatus.ReadyForPickup.ToString());

            if (responseDto is not null && responseDto.IsSuccess)
            {
                TempData["success"] = "Status updated successfully";
                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }
            
            return View();
        }

        [HttpPost("CompleteOrder")]
        public async Task<IActionResult> CompleteOrder(int orderId)
        {
            var responseDto = await _orderService.UpdateOrderStatus(orderId, StaticTypes.OrderStatus.Completed.ToString());

            if (responseDto is not null && responseDto.IsSuccess)
            {
                TempData["success"] = "Status updated successfully";
                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }

            return View();
        }

        [HttpPost("CancelOrder")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var responseDto = await _orderService.UpdateOrderStatus(orderId, StaticTypes.OrderStatus.Cancelled.ToString());

            if (responseDto is not null && responseDto.IsSuccess)
            {
                TempData["success"] = "Status updated successfully";
                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }

            return View();
        }
    }
}
