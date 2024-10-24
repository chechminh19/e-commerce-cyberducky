using Application.ServiceResponse;
using Application.ViewModels;
using Domain.Entities;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface IOrderService
    {
        //Task<ResultModel> VerifyPaymentWebhookData(WebhookType webhookBody);     
        Task<ServiceResponse<string>> UpdateProductQuantitiesBasedOnCart(Order order);
        Task<ServiceResponse<string>> PaymentOrder(int orderId);
        Task<bool> UpdateOrderCodePay(int orderId, int codePay);
        Task<ServiceResponse<CreateOrderDTO>> GetAllOrderCustomerCart(int userId);
        Task<ServiceResponse<string>> AddProductToOrderAsync(int userId, int productId);
        Task<ServiceResponse<bool>> UpdateOrderQuantity(int orderId, int productId, int quantity);
        Task<ServiceResponse<bool>> RemoveProductFromCart(int orderId, int productId);
    }
}
