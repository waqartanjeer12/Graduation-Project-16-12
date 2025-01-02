using ECommerceCore.DTOs.Color;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECommerceCore.DTOs.Product
{
    public class ProductReadForCreateDTO
    {
        [Required(ErrorMessage = "يرجى ادخال اسم المنتج")]
        public string Name { get; set; }

        [Required(ErrorMessage = "يرجى ادخال وصف المنتج")]
        public string Description { get; set; }

        [Required(ErrorMessage = "يرجى اختيار فئة المنتج")]
        public string CategoryName { get; set; }
        [Required(ErrorMessage = "يرجى تحميل الصورة الرئيسية للمنتج")]
        public string MainImageUrl { get; set; }
        public List<string> AdditionalImageUrls { get; set; }

        [Required(ErrorMessage = "يرجى إدخال السعر الخاص بالمنتج")]
        [Range(0.01, double.MaxValue, ErrorMessage = "يجب أن يكون السعر قيمة موجبة")]
        [Precision(18, 2)] // Specify precision and scale for Price

        public decimal Price { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "يرجى إدخال سعر لا يقل عن 0")]
        [Precision(18, 2)] // Specify precision and scale for Price
        public decimal? OriginalPrice { get; set; }

        [Required(ErrorMessage = "يرجى تحديد كمية المخزون المتاحة للمنتج")]
        [Range(0, int.MaxValue, ErrorMessage = "يجب أن تكون كمية المخزون 0 أو أكبر")]
        public int Inventory { get; set; }


        public List<ColorReadForUserDTO> ColorDetails { get; set; }
    }
}