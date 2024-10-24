﻿using Application.IRepository;
using Application.IService;
using Application.ServiceResponse;
using Application.Utils;
using Application.ViewModels;
using AutoMapper;
using Domain.Entities;
using Net.payOS.Types;
using Net.payOS;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IMapper _mapper;
        private readonly IProductRepo _productRepo;
        public OrderService(IOrderRepo orderRepo, IMapper mapper, IProductRepo product)
        {
            _orderRepo = orderRepo ?? throw new ArgumentNullException(nameof(orderRepo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _productRepo = product;
        }

        public async Task<ServiceResponse<string>> AddProductToOrderAsync(int userId, int productId)
        {
            var response = new ServiceResponse<string>();
            try
            {
                var checkUserOrder = await _orderRepo.CheckUserWithOrder(userId);
                if (checkUserOrder == null || checkUserOrder.Status == 2)
                {
                    Order newOrder = new Order
                    {
                        UserId = userId,
                        Status = 1,
                        OrderDetails = new List<OrderDetails>()
                    };
                    OrderDetails newOrderDetail = new OrderDetails
                    {
                        ProductId = productId,
                        QuantityProduct = 1,
                        Price = _productRepo.GetProductPriceById(productId),
                    };
                    newOrder.OrderDetails.Add(newOrderDetail);
                    await _orderRepo.AddOrder(newOrder);
                    response.Success = true;
                    response.Message = "Add Product successfully when have no order already for user";
                }
                else
                {
                    var existingOrder = checkUserOrder.OrderDetails.FirstOrDefault(od => od.ProductId == productId);
                    if (existingOrder != null)
                    {
                        existingOrder.QuantityProduct += 1;
                        await _orderRepo.UpdateOrderDetail(existingOrder);
                        response.Success = true;
                        response.Message = "add duplicate success";
                    }
                    else
                    {
                        OrderDetails existingOrderDetail = new OrderDetails
                        {
                            OrderId = checkUserOrder.Id,
                            ProductId = productId,
                            QuantityProduct = 1,
                            Price = _productRepo.GetProductPriceById(productId),
                        };
                        await _orderRepo.AddOrderDetail(existingOrderDetail);
                        response.Success = true;
                        response.Message = "Add product successfully when already order for user";
                    }
                }
            }
            catch (DbException e)
            {
                response.Success = false;
                response.Message = "Database error occurred.";
                response.ErrorMessages = new List<string> { e.Message };
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = "Error";
                response.ErrorMessages = new List<string> { e.Message, e.StackTrace };
            }

            return response;
        }

        public async Task<ServiceResponse<CreateOrderDTO>> GetAllOrderCustomerCart(int userId)
        {
            var response = new ServiceResponse<CreateOrderDTO>();
            try
            {
                var listDetail = await _orderRepo.GetAllOrderCart(userId);
                if (listDetail == null)
                {
                    response.Success = true;
                    response.Message = "No order here";
                    response.Data = null;
                    response.PriceTotal = 0;
                    return response;
                }

                var productList = new List<ProductToCreateOrderDTO>();

                foreach (var detail in listDetail)
                {
                    var productDto = new ProductToCreateOrderDTO
                    {
                        ProductId = detail.ProductId,
                        NameProduct = detail.Product.NameProduct,
                        DescriptionProduct = detail.Product.DescriptionProduct,
                        Price = detail.Product.Price,
                        Quantity = detail.QuantityProduct,
                        NameMaterial = detail.Product.Material.NameMaterial,
                        NameTypeProduct = detail.Product.TypeProduct.NameType,
                        NameColor = detail.Product.Color.NameColor,
                        ImageUrl = detail.Product.ProductImages.FirstOrDefault()?.ImageUrl,
                        OrderId = detail.OrderId,
                    };
                    productList.Add(productDto);
                }

                double priceTotal = productList.Sum(productDto => productDto.Price * productDto.Quantity);
                response.Success = true;
                response.Data = new CreateOrderDTO { Product = productList, PriceTotal = priceTotal };
                return response;
            }
            catch (DbException e)
            {
                response.Success = false;
                response.Message = "Database error occurred.";
                response.ErrorMessages = new List<string> { e.Message };
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = "Error";
                response.ErrorMessages = new List<string> { e.Message, e.StackTrace };
            }

            return response;
        }

        public async Task<ServiceResponse<string>> PaymentOrder(int orderId)
        {
            var serviceResponse = new ServiceResponse<string>();
            try
            {
                Order order = await _orderRepo.GetOrderByIdProcessingToPay(orderId, Enums.OrderCart.Process);

                if (order == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "not found orderId";
                }
                else
                {
                    order.Status = (byte)Enums.OrderCart.Completed;                  
                    DateTime utcNow = DateTime.UtcNow;
                    TimeZoneInfo gmtPlus7 = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                    DateTime gmtPlus7Now = TimeZoneInfo.ConvertTimeFromUtc(utcNow, gmtPlus7);
                    order.PaymentDate = gmtPlus7Now;

                    var updateResponse = await UpdateProductQuantitiesBasedOnCart(order);
                    if (updateResponse.Success == false)
                    {
                        serviceResponse.Success = false;
                        serviceResponse.Message = updateResponse.Message;
                        serviceResponse.ErrorMessages = updateResponse.ErrorMessages;
                        return serviceResponse;
                    }
                    await _orderRepo.SaveChangesAsync();
                    //await transaction.CommitAsync();
                    var orderEmailDto = new ShowOrderSuccessEmailDTO
                    {
                        OrderId = order.Id,
                        UserName = order.User.FullName,
                        PaymentDate = order.PaymentDate.Value,
                        OrderItems = order.OrderDetails.Select(od => new OrderItemEmailDto
                        {
                            ProductName = od.Product.NameProduct,
                            Quantity = od.QuantityProduct,
                            Price = od.Price
                        }).ToList()
                    };
                    // Send payment success email
                    var userEmail = order.User?.Email; // Assuming the Order object has a User property with an Email
                    if (!string.IsNullOrEmpty(userEmail))
                    {
                        var emailSent = await Utils.SendMail.SendOrderPaymentSuccessEmail(orderEmailDto, userEmail);

                        if (emailSent)
                        {
                            serviceResponse.Success = true;
                            serviceResponse.Message = "Payment successful and email sent.";
                        }
                        else
                        {
                            serviceResponse.Success = true;
                            serviceResponse.Message = "Payment successful but email sending failed.";
                        }
                    }
                    else
                    {
                        serviceResponse.Success = true;
                        serviceResponse.Message = "Payment successful but no user email found.";
                    }
                }
            }
            catch (DbException e)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Database error occurred.";
                serviceResponse.ErrorMessages = new List<string> { e.Message };
            }
            catch (Exception e)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Error error occurred.";
                serviceResponse.ErrorMessages = new List<string> { e.Message };
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<bool>> RemoveProductFromCart(int orderId, int productId)
        {
            var response = new ServiceResponse<bool>();

            var order = await _orderRepo.GetOrderWithDetailsAsync(orderId);
            if (order == null)
            {

                response.Success = false;
                response.Message = "Order not found.";
                return response;
            }

            var orderDetail = order.OrderDetails.FirstOrDefault(od => od.ProductId == productId);
            if (orderDetail == null)
            {

                response.Success = false;
                response.Message = "Product not found in order.";
                return response;
            }

            order.OrderDetails.Remove(orderDetail);
            if (!order.OrderDetails.Any())
            {
                await _orderRepo.Delete(order);
                response.Message = "Order and product removed successfully.";
            }
            else
            {
                await _orderRepo.Update(order);
                response.Message = "Product removed successfully.";
            }

            response.Data = true;
            response.Message = "Product removed successfully.";
            return response;
        }

        public async Task<bool> UpdateOrderCodePay(int orderId, int codePay)
        {
            var order = await _orderRepo.GetOrderByIdProcessingToPay(orderId, Enums.OrderCart.Process);  // Lấy order từ DB
            if (order != null)
            {
                order.CodePay = codePay;  // Cập nhật mã CodePay
                await _orderRepo.UpdateOrderCodePay(order);  // Lưu cập nhật vào DB
                return true;
            }
            return false;
        }

        public async Task<ServiceResponse<bool>> UpdateOrderQuantity(int orderId, int productId, int quantity)
        {
            var response = new ServiceResponse<bool>();

            var order = await _orderRepo.GetOrderWithDetailsAsync(orderId);
            if (order == null)
            {
                response.Success = false;
                response.Message = "Order not found.";
                return response;
            }

            var orderDetail = order.OrderDetails.FirstOrDefault(od => od.ProductId == productId);
            if (orderDetail == null)
            {
                response.Success = false;
                response.Message = "Product not found in order.";
                return response;
            }

            orderDetail.QuantityProduct = quantity;
            await _orderRepo.Update(order);

            response.Data = true;
            response.Message = "Quantity updated successfully.";
            return response;
        }

        public async Task<ServiceResponse<string>> UpdateProductQuantitiesBasedOnCart(Order order)
        {
            var serviceResponse = new ServiceResponse<string>();

            try
            {
                var orderDetails = await _orderRepo.GetOrderDetailsByOrderId(order.Id);
                if (orderDetails == null || !orderDetails.Any())
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Order not found or no items in cart.";
                    return serviceResponse;
                }

                var productUpdates = orderDetails
                 .GroupBy(detail => detail.ProductId)
                 .Select(group => new ProductUpdate
                 {
                     ProductId = group.Key,
                     QuantityChange = -group.Sum(detail => detail.QuantityProduct)
                 })
                 .ToList();

                await _productRepo.UpdateProductQuantities(productUpdates);

                serviceResponse.Success = true;
                serviceResponse.Message = "Product quantities updated successfully.";
            }
            catch (DbException e)
            {
                serviceResponse.Success = false;
                serviceResponse.ErrorMessages = new List<string> { e.Message };
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.ErrorMessages = new List<string> { ex.Message, ex.StackTrace };
            }

            return serviceResponse;
        }       
    }    
}
