using System.Collections.Generic;

namespace ECommerceCore.DTOs.Cart
{
    public class CartReadAddItemsToCartDTO
    {
        public int ItemId { get; set; }
        public CartReadProducts items { get; set; }
        public int Quantity { get; set; }
        public bool IsInventorySufficient { get; set; }
    }
}