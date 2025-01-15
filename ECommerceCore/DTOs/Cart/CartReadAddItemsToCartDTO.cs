using System.Collections.Generic;

namespace ECommerceCore.DTOs.Cart
{
    public class CartReadAddItemsToCartDTO
    {
        public int ItemId { get; set; }
        public CartReadProducts products { get; set; }
        public int Quantity { get; set; }
        
    }
}