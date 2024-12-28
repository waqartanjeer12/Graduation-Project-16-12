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

        [Required(ErrorMessage = "يرجى ادخال فئة المنتج")]
        public string CategoryName { get; set; }

        public string MainImageUrl { get; set; }
        public List<string> AdditionalImageUrls { get; set; }

        [Required(ErrorMessage = "السعر مطلوب")]
        [Range(0, double.MaxValue, ErrorMessage = "يجب أن يكون السعر أكبر من أو يساوي 0.")]
        [Precision(18, 2)] // Specify precision and scale for Price
        public decimal Price { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "يجب أن يكون السعر الأصلي أكبر من أو يساوي 0.")]
        [Precision(18, 2)] // Specify precision and scale for Price
        public decimal? OriginalPrice { get; set; }

        [Required(ErrorMessage = "يرجى ادخال مخزون المنتج")]
        public int Inventory { get; set; }


        public List<ColorReadForUserDTO> ColorDetails { get; set; }
    }
}