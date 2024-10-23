using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public virtual User? User { get; set; }
        public DateTime? PaymentDate { get; set; }
        public byte Status { get; set; }
        public int? CodePay { get; set; }

        public ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
