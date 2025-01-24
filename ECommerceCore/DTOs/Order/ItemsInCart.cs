using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.DTOs.Order
{
    public  class ItemsInCart
    {
        public int CartItemId {  get; set; }
        public String CartMainImageUrl { get; set; }
        public decimal cartItemPrice { get; set; }
        public int quantity { get; set; }
    }
}
