using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.DTOs.Cart
{
    public class CartGetAllItemsDTO
    {
        public int CartId { get; set; }
        public List<CartReadAddItemsToCartDTO> items { get; set; }

        public decimal totalPrice { get; set; }
        public decimal totalOriginalPrice { get; set; }

    }
}
