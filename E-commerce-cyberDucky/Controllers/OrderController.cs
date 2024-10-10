using Application.IService;
using Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce_cyberDucky.Controllers
{
    [EnableCors("Allow")]
    [Route("api/orders")]
    [ApiController]
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpPost("{userid}/{productid}")]
        public async Task<IActionResult> AddProductToOrder(int userid, int productid)
        {
            var result = await _orderService.AddProductToOrderAsync(userid, productid);
            if (!result.Success) return BadRequest(result);

            return Ok(result);
        }
        [HttpGet("customer/{userid}")]
        public async Task<IActionResult> GetAllOrderCartCustomer(int userid)
        {
            var result = await _orderService.GetAllOrderCustomerCart(userid);
            if (!result.Success) return BadRequest(result);

            return Ok(result);
        }
        [HttpPut("update-quantity")]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateQuantityRequest request)
        {
            var result = await _orderService.UpdateOrderQuantity(request.OrderId, request.ProductId, request.Quantity);
            if (!result.Success) return BadRequest(result);

            return Ok(result);
        }
        [HttpDelete("remove-product/{orderId}/{productId}")]
        public async Task<IActionResult> RemoveProduct(int orderId, int productId)
        {
            var result = await _orderService.RemoveProductFromCart(orderId, productId);
            if (!result.Success) return BadRequest(result);

            return Ok(result);
        }
    }
}
