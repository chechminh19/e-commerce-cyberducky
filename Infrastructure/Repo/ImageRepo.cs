using Application.IRepository;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repo
{
    public class ImageRepo : GenericRepo<ProductImages>, IImageRepo
    {
        private readonly AppDbContext _dbContext;

        public ImageRepo(AppDbContext context) : base(context)
        {
            _dbContext = context;
        }

        public async Task<ProductImages> GetImageInforById(int id)
        {
            return await _dbContext.ProductImage.FindAsync(id);
        }

        public async Task<IEnumerable<ProductImages>> GetAllImageInfors()
        {
            return _dbContext.ProductImage.ToList();
        }
      
    }
}
