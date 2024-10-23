using Application.Enums;
using Application.IRepository;
using Application.IService;
using Azure;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Types;
using Response = E_commerce_cyberDucky.Type.Response;
namespace E_commerce_cyberDucky.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/payment")]
    [ApiController]
    public class PaymentController : BaseController
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
        [HttpPost("payos_transfer_handler")]
        public  async Task<IActionResult> payOSTransferHandler(WebhookType body)
        {
            try
            {
                WebhookData data = _payOS.verifyPaymentWebhookData(body);
                int codePay = (int)data.orderCode;
                var order = await _orderRepo.GetOrderByCodePayTransfer(codePay);
                if (order == null)
                {
                    return Ok(new Response(-1, "Order not found", null));
                }
                if (data.code == "00")
                {                    
                    var result = await _orderService.PaymentOrder(order.Id);
                    if(result.Success == false)
                    {
                        return Ok(new Response(-1, "fail", null));
                    }
                    return Ok(new Response(0, "Ok", null));
                }
                return Ok(new Response(-1, "fail", null));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Ok(new Response(-1, "fail", null));
            }

        }
    }
}
