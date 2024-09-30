using Application.IRepository;
using Application.IService;
using Application.ServiceResponse;
using Application.ViewModels;
using AutoMapper;
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
