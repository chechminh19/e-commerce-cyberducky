using Application.IRepository;
using Application.Utils;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Repo
{
    public class ProductRepo :  GenericRepo<Product> , IProductRepo
    {
        private readonly AppDbContext _dbContext;
        public ProductRepo(AppDbContext context) : base(context)
        {
            _dbContext = context;
        }

        public async Task AddProduct(Product product)
        {
            try
            {
                if (product == null)
                {
                    throw new ArgumentNullException(nameof(product));
                }

                await _dbContext.Products.AddAsync(product);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the product.", ex);
            }
        }

        public Task DeleteProduct(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Product>> GetAllProduct()
        {
            return await _dbContext.Products
               .Include(p => p.ProductImages)
               .AsNoTracking()
               .ToListAsync();
        }

        public async Task<Product> GetProductById(int id)
        {
            var product = await _dbContext.Products
                .Include(p => p.ProductImages)
                .Include(p => p.Color)
                 .Include(p => p.Material)
                 .Include(p => p.TypeProduct)
                .AsNoTracking() // option for read-only data
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                throw new KeyNotFoundException($"Product with id {id} not found.");
            }

            return product;
        }

        public Task<Product> GetProductByIdToOrder(int id)
        {
            throw new NotImplementedException();
        }

        public double GetProductPriceById(int productId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetTotalProductsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Product>> GetTypeProduct(string idType)
        {
            return await _dbContext.Products.Where(a => a.TypeProductId.ToString() == idType)
              .Include(p => p.ProductImages)
              .AsNoTracking()
              .ToListAsync();
        }

        public Task UpdateProduct(Product product)
        {
            throw new NotImplementedException();
        }

        public Task UpdateProductQuantities(IEnumerable<ProductUpdate> products)
        {
            throw new NotImplementedException();
        }
    }
}
