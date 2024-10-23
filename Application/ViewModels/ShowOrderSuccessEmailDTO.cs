using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels
{
    public class ShowOrderSuccessEmailDTO
    {
        public int OrderId { get; set; }
        public string UserName { get; set; }
        public DateTime PaymentDate { get; set; }
        public List<OrderItemEmailDto> OrderItems { get; set; }
        public double TotalPrice => OrderItems?.Sum(item => item.TotalPrice) ?? 0;
    }
}
