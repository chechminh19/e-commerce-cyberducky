﻿using Application.IRepository;
using Application.IService;
using Application.ServiceResponse;
using Application.ViewModels;
using AutoMapper;
using Domain.Entities;
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
    }
}
