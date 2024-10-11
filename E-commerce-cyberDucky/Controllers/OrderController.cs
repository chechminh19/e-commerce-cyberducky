using Application.IService;
using Application.ViewModels;
using Azure;
using E_commerce_cyberDucky.Type;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Types;
using Response = E_commerce_cyberDucky.Type.Response;

namespace E_commerce_cyberDucky.Controllers
{
    [EnableCors("Allow")]
    [Route("api/orders")]
    [ApiController]
    public class OrderController : BaseController
    {
        private readonly PayOS _payOS;
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService, PayOS payOS)
        {
            _orderService = orderService;
            _payOS = payOS;
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreatePaymentLink(CreatePaymentLinkRequest body)
        {
            try
            {
                int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));
                ItemData item = new ItemData(body.productName, 1, body.price);
                List<ItemData> items = new List<ItemData>();
                items.Add(item);
                PaymentData paymentData = new PaymentData(orderCode, body.price, body.description, items, body.cancelUrl, body.returnUrl);

                CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);

                return Ok(new Response(0, "success", createPayment));
            }
            catch (System.Exception exception)
            {
                Console.WriteLine(exception);
                return Ok(new Response(-1, "fail", null));
            }
        }
        [HttpPut("{orderId}")]
        public async Task<IActionResult> CancelOrder([FromRoute] int orderId)
        {
            try
            {
                PaymentLinkInformation paymentLinkInformation = await _payOS.cancelPaymentLink(orderId);
                return Ok(new Response(0, "Ok", paymentLinkInformation));
            }
            catch (System.Exception exception)
            {

                Console.WriteLine(exception);
                return Ok(new Response(-1, "fail", null));
            }

        }
        [HttpPost("confirm-webhook")]
        public async Task<IActionResult> ConfirmWebhook(ConfirmWebhook body)
        {
            try
            {
                await _payOS.confirmWebhook(body.webhook_url);
                return Ok(new Response(0, "Ok", null));
            }
            catch (System.Exception exception)
            {

                Console.WriteLine(exception);
                return Ok(new Response(-1, "fail", null));
            }

        }
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrder([FromRoute] int orderId)
        {
            try
            {
                PaymentLinkInformation paymentLinkInformation = await _payOS.getPaymentLinkInformation(orderId);
                return Ok(new Response(0, "Ok", paymentLinkInformation));
            }
            catch (System.Exception exception)
            {

                Console.WriteLine(exception);
                return Ok(new Response(-1, "fail", null));
            }

        }
        /// <summary>
        /// add product to order
        /// </summary>      
        [AllowAnonymous]
        [HttpPost("{userid}/{productid}")]
        public async Task<IActionResult> AddProductToOrder(int userid, int productid)
        {
            var result = await _orderService.AddProductToOrderAsync(userid, productid);
            if (!result.Success) return BadRequest(result);

            return Ok(result);
        }
        /// <summary>
        /// get all product of customer
        /// </summary>      
        [AllowAnonymous]
        [HttpGet("customer/{userid}")]
        public async Task<IActionResult> GetAllOrderCartCustomer(int userid)
        {
            var result = await _orderService.GetAllOrderCustomerCart(userid);
            if (!result.Success) return BadRequest(result);

            return Ok(result);
        }
        /// <summary>
        /// update quantity product cart
        /// </summary>      
        [AllowAnonymous]
        [HttpPut("update-quantity")]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateQuantityRequest request)
        {
            var result = await _orderService.UpdateOrderQuantity(request.OrderId, request.ProductId, request.Quantity);
            if (!result.Success) return BadRequest(result);

            return Ok(result);
        }
        /// <summary>
        /// remove quantity product cart
        /// </summary>      
        [AllowAnonymous]
        [HttpDelete("remove-product/{orderId}/{productId}")]
        public async Task<IActionResult> RemoveProduct(int orderId, int productId)
        {
            var result = await _orderService.RemoveProductFromCart(orderId, productId);
            if (!result.Success) return BadRequest(result);

            return Ok(result);
        }
    }
}
