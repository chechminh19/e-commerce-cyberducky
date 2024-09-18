using Application.Utils;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IRepository
{
    public interface IProductRepo
    {
        Task<Product> GetProductById(int id);
        Task<IEnumerable<Product>> GetAllProduct();
        Task<IEnumerable<Product>> GetTypeProduct(string idType);
        Task AddProduct(Product product);
        Task UpdateProduct(Product product);
        Task DeleteProduct(int id);
        Task<Product> GetProductByIdToOrder(int id);
        double GetProductPriceById(int productId);
        Task<int> GetTotalProductsAsync();
        Task UpdateProductQuantities(IEnumerable<ProductUpdate> products);
    }
}
