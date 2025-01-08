using ECommerceCore.DTOs.Color;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.DTOs.Cart
{
    public class CartReadProducts
    {
        [Required]
        public int ProductId { get; set; }
        [Required(ErrorMessage = "يرجى ادخال اسم المنتج")]
        public string Name { get; set; }

        [Required(ErrorMessage = "يرجى ادخال وصف المنتج")]
        public string Description { get; set; }

       
        [Required(ErrorMessage = "يرجى تحميل الصورة الرئيسية للمنتج")]
        public string MainImageUrl { get; set; }
        

        [Required(ErrorMessage = "يرجى إدخال السعر الخاص بالمنتج")]
        [Range(0.01, double.MaxValue, ErrorMessage = "يجب أن يكون السعر قيمة موجبة")]
        [Precision(18, 2)] // Specify precision and scale for Price

        public decimal Price { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "يرجى إدخال سعر لا يقل عن 0")]
        [Precision(18, 2)] // Specify precision and scale for Price
        public decimal? OriginalPrice { get; set; }
        public ColorReadDTO ColorDetails { get; set; }

        
  
    }
}
