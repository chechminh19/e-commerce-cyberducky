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
        [HttpPost("payos_transfer_handler")]
        public IActionResult payOSTransferHandler(WebhookType body)
        {
            try
            {               
                WebhookData data = _payOS.verifyPaymentWebhookData(body);
                if (data == null)
                {
                    return BadRequest(new Response(-1, "Invalid data from webhook", null));
                }
                //var aa = data.accountNumber;
                //var bb = data.paymentLinkId;
                //var cc = data.transactionDateTime;
                //var dd = data.virtualAccountNumber;
                string responseCode = data.code;
                var orderCode = (int)data.orderCode;
                var order =  _orderRepo.GetOrderByCodePayTransfer(orderCode);
                //if (orderCode != null || responseCode == "00" || data.desc == "success")
                if(data.description == "Ma giao dich thu nghiem" || data.description == "VQRIO123")
                {
                    var result =  _orderService.PaymentOrder(order.Id);
                    if(result != null)
                    {
                        return Ok(new Response(0, "Ok", null));
                    }
                    return Ok(new Response(0, "Ok", null));
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
