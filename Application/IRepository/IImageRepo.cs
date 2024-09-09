using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IRepository
{
    public interface IImageRepo : IGenericRepo<ProductImages>
    {
        Task<ProductImages> GetImageInforById(int id);
        Task<IEnumerable<ProductImages>> GetAllImageInfors();

        //Task DeleteProductImage(int id);
    }
}
