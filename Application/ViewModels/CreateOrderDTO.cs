using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels
{
    public class CreateOrderDTO
    {
        public List<ProductToCreateOrderDTO> Product { get; set; }
        public double PriceTotal { get; set; }
    }
}
