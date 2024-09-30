using Application.ServiceResponse;
using Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface IProductService
    {
        Task<ServiceResponse<PaginationModel<ProductDTO>>> GetAllProductsAsync(int page, int pageSize, string search, string sort);
        Task<ServiceResponse<ProductDTO>> GetProductByIdAsync(int id);
        Task<ServiceResponse<int>> CreateProductAsync(CreateProductDTO cproduct);
        Task<ServiceResponse<PaginationModel<ProductDTO>>> GetAllProductType(int page, int pageSize, string search, string sort, string idType);
        //Task<ServiceResponse<string>> UpdateProductAsync(CreateProductDTO cproduct, int zodiacId);
        //Task<ServiceResponse<string>> DeleteProductAsync(int id);
        //Task<ServiceResponse<ProductStatisticsDTO>> GetProductStatisticsAsync();
    }
}
