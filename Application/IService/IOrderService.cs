using Application.ServiceResponse;
using Application.ViewModels;
using Application.ViewModels.UserDTO;
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
        Task<ResultModel> UpdateOrderPayment(long orderCode);     
        Task<ServiceResponse<string>> UpdateProductQuantitiesBasedOnCart(Order order);
        Task<bool> UpdateOrderCodePay(int orderId, int codePay);
        Task<ServiceResponse<CreateOrderDTO>> GetAllOrderCustomerCart(int userId);
        Task<ServiceResponse<string>> AddProductToOrderAsync(int userId, int productId);
        Task<ServiceResponse<bool>> UpdateOrderQuantity(int orderId, int productId, int quantity);
        Task<ServiceResponse<bool>> RemoveProductFromCart(int orderId, int productId);
        Task<List<OrderForAdminDTO>> GetAllOrdersForAdmin();
    }
}
