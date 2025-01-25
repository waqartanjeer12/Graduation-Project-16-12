using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ECommerceCore.DTOs.Order
{
    public class ItemsInCart
    {
        public int OrderItemId { get; set; } // Added OrderItemId
        
        public string CartMainImageUrl { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "يجب أن يكون السعر قيمة موجبة")]
        [Precision(18, 2)]
        public decimal CartItemPrice { get; set; }
        public int Quantity { get; set; }
    }
}