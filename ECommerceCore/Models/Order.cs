using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public DateTime orderDate { get; set; }
        public String orderStatus { get; set; }
        public String orderStatusDetails { get; set; }

        public string FName { get; set; }
        public string LName { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Area { get; set; }
        public decimal totalPriceBeforeShipping { get; set; }
        public decimal shippingPrice { get; set; }
        public decimal totalPrice { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
       

        public ICollection<CartItem> CartItems { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
