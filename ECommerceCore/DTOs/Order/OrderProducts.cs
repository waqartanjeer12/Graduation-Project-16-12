using ECommerceCore.DTOs.Color;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.DTOs.Order
{
    public class OrderProducts
    {
        [Required]
        public int Id { get; set; }
        [Required(ErrorMessage = "يرجى ارجاع صورة المنتج")]
        public string MainImageUrl { get; set; }
        [Required(ErrorMessage = "يرجى ارجاع اسم المنتج")]
        public string Name { get; set; }
        [Required(ErrorMessage = "يرجى ارجاع الكمية")]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "يرجى ارجاع سعر القطعة الواحدة")]
        public decimal OnePiecePrice { get; set; }
        [Required(ErrorMessage = "يرجى ارجاع السعر الكلي")]
        public decimal totalPricewithQuantit { get; set; }
        [Required(ErrorMessage = "يرجى ارجاع اللون")]
        public ColorReadDTO Color { get; set; }

    }
}
