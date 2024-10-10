using Application.ServiceResponse;
using Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface IOrderService
    {
        Task<ServiceResponse<CreateOrderDTO>> GetAllOrderCustomerCart(int userId);
        Task<ServiceResponse<string>> AddProductToOrderAsync(int userId, int productId);

    }
}
