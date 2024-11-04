using Application.IService;
using Application.ViewModels;
using Azure;
using Domain.Entities;
using E_commerce_cyberDucky.Type;
using MailKit.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net.payOS;
using Net.payOS.Types;
using System.Linq;
using Response = E_commerce_cyberDucky.Type.Response;

namespace E_commerce_cyberDucky.Controllers
{
    [EnableCors("AllowAll")]
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
            var total = 0;
            try
            {
                var result = await _orderService.GetAllOrderCustomerCart(body.userId);
                if (!result.Success || result.Data == null)
                {
                    return Ok(new Response(-1, "No order found", null));
                }
                var productList = result.Data.Product;
                double totalPrice = result.Data.PriceTotal;
                int orderId = productList.FirstOrDefault()?.OrderId ?? 0; // Lấy orderId từ sản phẩm đầu tiên
                int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));
                var des = productList.FirstOrDefault().DescriptionProduct;
                bool isUpdated = await _orderService.UpdateOrderCodePay(orderId, orderCode);
                if (isUpdated == false)
                {
                    return Ok(new Response(-1, "Failed to update CodePay", null));
                }

                // Step 4: Prepare the list of items for the payment link
                List<ItemData> items = productList.Select(product => new ItemData(
                    product.NameProduct,
                    product.Quantity,
                    (int)product.Price
                )).ToList();
                total = (int)totalPrice;
                PaymentData paymentData = new PaymentData(orderCode, total, des, items, body.cancelUrl,body.returnUrl);

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
        public async Task<IActionResult> GetOrder([FromRoute] long orderId)
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
        /// <summary>
        /// get all order of customer for admin
        /// </summary>      
        [HttpGet("all")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersForAdmin();
            return Ok(orders);
        }
    }
}
