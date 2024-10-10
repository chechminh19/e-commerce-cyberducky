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
        Task<List<OrderDetails>> GetAllOrderCart(int userId);
        Task<Order> CheckUserWithOrder(int userId); 
        Task AddOrder(Order order);
        Task UpdateOrderDetail(OrderDetails orderDetail);
        Task AddOrderDetail(OrderDetails orderDetail);
    }
}
