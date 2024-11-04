using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.UserDTO
{
    public class OrderForAdminDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public DateTime? PaymentDate { get; set; }
        public byte Status { get; set; }
        public int? CodePay { get; set; }
        public double PriceTotal { get; set; }
    }
}
