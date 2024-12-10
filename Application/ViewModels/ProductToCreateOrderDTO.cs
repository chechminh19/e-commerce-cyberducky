using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels
{
    public class ProductToCreateOrderDTO
    {
        public int ProductId { get; set; }
        public string NameProduct { get; set; }
        public string DescriptionProduct { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string NameTypeProduct { get; set; }
        public string NameMaterial { get; set; }
        public string NameColor { get; set; }
        public string ImageUrl { get; set; }
        public int OrderId { get; set; }
    }
}
