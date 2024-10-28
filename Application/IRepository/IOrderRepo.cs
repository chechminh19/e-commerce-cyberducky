using Application.Enums;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IRepository
{
    public interface IOrderRepo : IGenericRepo<Order>
    {
        Task<Order> GetOrderByOrderCode(long orderCode);
        Task<ICollection<OrderDetails>> GetOrderDetailsByOrderId(int orderId);
        Task<Order> GetOrderByCodePayTransfer(int codePay);
        Task UpdateOrderCodePay(Order order);
        Task<Order> GetOrderByIdProcessingToPay(int orderId, OrderCart status);
        Task<List<OrderDetails>> GetAllOrderCart(int userId);
        Task<int> GetOrderIdByUserIdToUpdateQR(int userId);
        Task<Order> CheckUserWithOrder(int userId); 
        Task AddOrder(Order order);
        Task UpdateOrderPayment(Order order);
        Task SaveChangesAsync();
        Task UpdateOrderDetail(OrderDetails orderDetail);
        Task AddOrderDetail(OrderDetails orderDetail);
        Task<Order?> GetOrderWithDetailsAsync(int orderId);
        Task Delete(Order order);
    }
}
