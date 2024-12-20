﻿using Application.Enums;
using Application.IRepository;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        public async Task Delete(Order order)
        {
            _dbContext.Orders.Remove(order);
            await _dbContext.SaveChangesAsync();
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

        public async Task<List<Order>> GetAllOrdersForAdmin()
        {
            return await _dbContext.Orders
          .Include(o => o.OrderDetails)
          .Include(o => o.User)
          .ToListAsync();
        }

        public Task<Order> GetOrderByCodePayTransfer(int codePay)
        {
           return  _dbContext.Orders
       .Include(o => o.OrderDetails)
       .FirstOrDefaultAsync(o => o.CodePay == codePay);
        }

        public async Task<Order> GetOrderByIdProcessingToPay(int orderId, OrderCart status)
        {
            return await _dbContext.Orders
        .Where(o => o.Id == orderId && o.Status == (byte)OrderCart.Process)
        .FirstOrDefaultAsync();
        }

        public async Task<Order> GetOrderByOrderCode(long orderCode)
        {
            if (orderCode > int.MaxValue || orderCode < int.MinValue)
            {
                return null;
            }

            int? codePayToCheck = (int?)orderCode;

            try
            {
                var order =  _dbContext.Orders
                    .FirstOrDefault(x => x.CodePay == codePayToCheck);

                return order;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while fetching order: {ex.Message}");
                return null;
            }
        }


        public async Task<ICollection<OrderDetails>> GetOrderDetailsByOrderId(int orderId)
        {
            try
            {
                Console.WriteLine($"Fetching order details for OrderId: {orderId}");
                return await _dbContext.OrderDetail.Where(od => od.OrderId == orderId).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while fetching order details: {ex.Message}");
                return new List<OrderDetails>();
            }

        }
        public async Task<int> GetOrderIdByUserIdToUpdateQR(int userId)
        {
            var order = await _dbContext.Orders
        .FirstOrDefaultAsync(o => o.UserId == userId && o.Status == 1);
            return order?.Id ?? 0;
        }

        public async Task<Order?> GetOrderWithDetailsAsync(int orderId)
        {
            return await _dbContext.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateOrderCodePay(Order order)
        {
            _dbContext.Orders.Update(order);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateOrderDetail(OrderDetails orderDetail)
        {
            _dbContext.OrderDetail.Update(orderDetail);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateOrderPayment(Order order)
        {
            _dbContext.Orders.Update(order);
            await _dbContext.SaveChangesAsync();
        }
    }
}
