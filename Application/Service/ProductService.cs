using Application.IRepository;
using Application.IService;
using Application.ServiceResponse;
using Application.Utils;
using Application.ViewModels;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepo _productRepo;
        private readonly IMapper _mapper;
        public ProductService(IProductRepo productRepo, IMapper mapper)
        {
            _productRepo = productRepo ?? throw new ArgumentNullException(nameof(productRepo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ServiceResponse<int>> CreateProductAsync(CreateProductDTO cproduct)
        {
            var response = new ServiceResponse<int>();

            try
            {
                var newProduct = MapToEntityCreate(cproduct);
                newProduct.Id = 0;

                await _productRepo.AddProduct(newProduct);
                response.Data = newProduct.Id;
                response.Success = true;
                response.Message = "Product created successfully";           
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to create product: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<PaginationModel<ProductDTO>>> GetAllProductsAsync(int page, int pageSize, 
            string search, string sort)
        {
            var response = new ServiceResponse<PaginationModel<ProductDTO>>();

            try
            {
                var products = await _productRepo.GetAllProduct();
                if (!string.IsNullOrEmpty(search))
                {
                    products = products.Where(p => p.NameProduct.Contains(search, StringComparison.OrdinalIgnoreCase));
                }
              
                products = sort.ToLower() switch
                {  // Sort by price in ascending order when "price" is specified
                    "price" => products.OrderBy(p => p.Price),
                    // Sort by name, material, color as required
                    "name" => products.OrderBy(p => p.NameProduct),
                    "material" => products.OrderBy(p => p.MaterialId),
                    "color" => products.OrderBy(p => p.ColorId),
                    // Default to sorting by Id in descending order (newest first) when no sort or other sort types
                    _ => products.OrderByDescending(p => p.Id)
                };
                var productDTOs = MapToDTO(products); // Map products to ProductDTO

                // Apply pagination
                var paginationModel = await Pagination.GetPaginationIENUM(productDTOs, page, pageSize);

                response.Data = paginationModel;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve products: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<PaginationModel<ProductDTO>>> GetAllProductType(int page, int pageSize, 
            string search, string sort, string idType)
        {
            var response = new ServiceResponse<PaginationModel<ProductDTO>>();

            try
            {
                var products = await _productRepo.GetTypeProduct(idType);
                if (!string.IsNullOrEmpty(search))
                {
                    products = products.Where(p => p.NameProduct.Contains(search, StringComparison.OrdinalIgnoreCase));
                }

                products = sort.ToLower() switch
                {  // Sort by price in ascending order when "price" is specified
                    "price" => products.OrderBy(p => p.Price),
                    // Sort by name, material, color as required
                    "name" => products.OrderBy(p => p.NameProduct),
                    "material" => products.OrderBy(p => p.MaterialId),
                    "color" => products.OrderBy(p => p.ColorId),
                    // Default to sorting by Id in descending order (newest first) when no sort or other sort types
                    _ => products.OrderByDescending(p => p.Id)
                };
                var productDTOs = MapToDTO(products); // Map products to ProductDTO

                // Apply pagination
                var paginationModel = await Pagination.GetPaginationIENUM(productDTOs, page, pageSize);

                response.Data = paginationModel;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve products: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<ProductDTO>> GetProductByIdAsync(int id)
        {
            var response = new ServiceResponse<ProductDTO>();

            try
            {
                var product = await _productRepo.GetProductById(id);
                if (product == null)
                {
                    response.Success = false;
                    response.Message = "Product not found";
                }
                else
                {
                    var productDTO = MapToDTO(product);
                    response.Data = productDTO;
                    response.Success = true;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve product: {ex.Message}";
            }

            return response;
        }

        private ProductDTO MapToDTO(Product product)
        {
            var productDTO = _mapper.Map<ProductDTO>(product);
            productDTO.ImageUrls = product.ProductImages?.Select(pi => pi.ImageUrl).ToList();
            return productDTO;
        }
        private IEnumerable<ProductDTO> MapToDTO(IEnumerable<Product> products)
        {
            return products.Select(MapToDTO);
        }
        private Product MapToEntityCreate(CreateProductDTO CreateProductDTO)
        {
            return _mapper.Map<Product>(CreateProductDTO);
        }
    }
}
