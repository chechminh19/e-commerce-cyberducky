using Application.IRepository;
using Application.Utils;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
namespace Infrastructure.Repo
{
    public class ProductRepo : GenericRepo<Product>, IProductRepo
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<ProductRepo> _logger;
        public ProductRepo(AppDbContext context, ILogger<ProductRepo> logger) : base(context)
        {
            _dbContext = context;
            _logger = logger;
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

        public async Task DeleteProduct(int id)
        {
            try
            {
                var product = await _dbContext.Products.FindAsync(id);
                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with id {id} not found.");
                }

                _dbContext.Products.Remove(product);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the product.", ex);
            }
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

        public async Task<Product> GetProductByIdToPay(int productId)
        {
            return await _dbContext.Products.FindAsync(productId);
        }

        public double GetProductPriceById(int productId)
        {
            return _dbContext.Products.FirstOrDefault(p => p.Id == productId)?.Price ?? 0;
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

        public async Task SaveChangesPay()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateProduct(Product product)
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateProductQuantities(IEnumerable<ProductUpdate> products)
        {
            // Make sure to log before doing anything with the context
            _logger.LogInformation("list product first: ");
            foreach (var product in products)
            {
                _logger.LogInformation($"ProductId: {product.ProductId}, QuantityChange: {product.QuantityChange}");
            }

            foreach (var update in products)
            {
                var product = await _dbContext.Products.FindAsync(update.ProductId);
                if (product != null)
                {
                    product.Quantity += update.QuantityChange;

                    if (product.Quantity < 0)
                    {
                        throw new InvalidOperationException($"Insufficient quantity for product ID: {product.Id}");
                    }

                    _dbContext.Products.Update(product);
                }
                else
                {
                    throw new InvalidOperationException($"Product not found with ID: {update.ProductId}");
                }
            }

            // Save changes asynchronously
            await _dbContext.SaveChangesAsync();
        }
    }
}
