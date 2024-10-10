using Application.Enums;
using Application.IRepository;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repo
{
    public class OrderRepo : GenericRepo<Order>, IOrderRepo
    {
        private readonly AppDbContext _dbContext;

        public OrderRepo(AppDbContext context) : base(context)
        {
            _dbContext = context;
        }

        public async Task AddOrder(Order order)
        {

            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddOrderDetail(OrderDetails orderDetail)
        {
            _dbContext.OrderDetail.Add(orderDetail);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Order> CheckUserWithOrder(int userId)
        {
            var order = await _dbContext.Orders.Include(p => p.OrderDetails)
                 .FirstOrDefaultAsync(p => p.UserId == userId && p.Status == (byte)OrderCart.Process);
            return order;
        }

        public async Task<List<OrderDetails>> GetAllOrderCart(int userId)
        {
            return _dbContext.Orders
                .Where(o => o.UserId == userId && o.Status == (byte)OrderCart.Process)
                .SelectMany(o => o.OrderDetails)
                .Include(od => od.Product)
                .ThenInclude(p => p.TypeProduct)
                
                .Include(od => od.Product)                      
                .ThenInclude(p => p.Material)
                
                .Include(od => od.Product)                            
                .ThenInclude(p => p.ProductImages)
                
                .Include(od => od.Product)
                .ThenInclude(p => p.Color)
                .ToList();
        }

        public async Task UpdateOrderDetail(OrderDetails orderDetail)
        {
            _dbContext.OrderDetail.Update(orderDetail);
            await _dbContext.SaveChangesAsync();
        }
    }
}
