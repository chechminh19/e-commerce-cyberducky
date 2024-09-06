using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        //public DbSet<Category> Category { get; set; }
        //public DbSet<CollectionProduct> CollectionProduct { get; set; }
        //public DbSet<Collections> Collection { get; set; }
        //public DbSet<Material> Material { get; set; }
        //public DbSet<Order> Order { get; set; }
        //public DbSet<OrderDetails> OrderDetail { get; set; }
        //public DbSet<Product> Product { get; set; }
        //public DbSet<User> User { get; set; }
        //public DbSet<Zodiac> Zodiac { get; set; }
        //public DbSet<Gender> Gender { get; set; }
        //public DbSet<ProductImage> ProductImage { get; set; }
        //public DbSet<ZodiacProduct> ZodiacProduct { get; set; }


    }
}
