using Application.IRepository;
using Application.IService;
using Application.ServiceResponse;
using Application.Utils;
using Application.ViewModels;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        private void MapCreateProductDTOToEntity(CreateProductDTO productDTO, Product existingProduct)
        {
            existingProduct.NameProduct = productDTO.NameProduct;
            existingProduct.DescriptionProduct = productDTO.DescriptionProduct;
            existingProduct.Price = productDTO.Price;
            existingProduct.Quantity = productDTO.Quantity;
            existingProduct.TypeProductId = productDTO.TypeProductId;
            existingProduct.MaterialId = productDTO.MaterialId;
            existingProduct.ColorId = productDTO.ColorId;
        }
        public async Task<ServiceResponse<string>> UpdateProductAsync(CreateProductDTO cproduct)
        {
            var response = new ServiceResponse<string>();

            try
            {
                // Validate the product DTO
                var validationContext = new ValidationContext(cproduct);
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(cproduct, validationContext, validationResults, true))
                {
                    var errorMessages = validationResults.Select(r => r.ErrorMessage);
                    response.Success = false;
                    response.Message = string.Join("; ", errorMessages);
                    return response;
                }

                // Retrieve the existing product from the repository
                var existingProduct = await _productRepo.GetProductById(cproduct.Id);
                if (existingProduct == null)
                {
                    response.Success = false;
                    response.Message = "Product not found";
                    return response;
                }

                // Map updated values from DTO to the existing entity
                MapCreateProductDTOToEntity(cproduct, existingProduct);

                // Update the product in the repository
                await _productRepo.UpdateProduct(existingProduct);              
                response.Data = "Product updated successfully";
                response.Success = true;
            }
            catch (Exception ex)
            {
                // Log the exception


                response.Success = false;
                response.Message = $"Failed to update product: {ex.Message}";
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
