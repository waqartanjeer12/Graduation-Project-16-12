using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ECommerceCore.DTOs.Cart
{
    public class CartAddItemsToCartDTO
    {

        [Required(ErrorMessage = "يرجى إدخال رقم المنتج")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "يرجى إدخال الكمية")]
        [Range(1, int.MaxValue, ErrorMessage = "الكمية يجب أن تكون على الأقل 1")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "يرجى إدخال اسم اللون")]
        public String ColorName { get; set; }

        public int UserId { get; set; }
    }
}
