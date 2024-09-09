using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ProductImages
    {
        [Key]
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
        public string? PublicId { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
