using Application.Enums;
using Application.IRepository;
using Application.IService;
using Application.ViewModels;
using Azure;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Types;
using Response = E_commerce_cyberDucky.Type.Response;
namespace E_commerce_cyberDucky.Controllers
{
    //[EnableCors("AllowAll")] 
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly PayOS _payOS;
        private readonly IOrderRepo _orderRepo;
        private readonly IOrderService _orderService;

        public PaymentController(PayOS payOS, IOrderRepo order, IOrderService service)
        {
             _payOS = payOS;
            _orderRepo = order;
            _orderService = service;
        }
        /// <summary>
        /// update order after pyament by self
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("payos_transfer_handler")]
        public async Task<IActionResult> payOSTransferHandler(WebhookType body)
        {
          
                try
                {
                    WebhookData data = _payOS.verifyPaymentWebhookData(body);                    
                    if (data.code == "00")
                    {
                        var result = await _orderService.UpdateOrderPayment(data.orderCode);
                        return Ok(new Response(0, "Ok", result));
                    }
                    return Ok(new Response(0, "Ok", null));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return Ok(new Response(-1, "fail", null));
                }
            
        }
    }
}
