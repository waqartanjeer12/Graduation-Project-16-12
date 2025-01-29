using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace ECommerceCore.DTOs.Product
{
    public class ProductUpdateDTO
    {
        [Required(ErrorMessage = "يرجى ادخال اسم المنتج")]
        public string Name { get; set; }
        public string? Description { get; set; }

        [Required(ErrorMessage = "يرجى اختيار فئة المنتج")]
        public string CategoryName { get; set; }

        public IFormFile? MainImage { get; set; }

        public List<IFormFile>? AdditionalImages { get; set; }

        [Required(ErrorMessage = "يرجى إدخال السعر الخاص بالمنتج")]
        [Range(0.01, double.MaxValue, ErrorMessage = "يجب أن يكون السعر قيمة موجبة")]
        [Precision(18, 2)] // Specify precision and scale for Price
        public decimal Price { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "يرجى إدخال سعر لا يقل عن 0")]
        [Precision(18, 2)] // Specify precision and scale for OriginalPrice
        public decimal? OriginalPrice { get; set; }

        [Required(ErrorMessage = "يرجى تحديد كمية المخزون المتاحة للمنتج")]
        [Range(0, int.MaxValue, ErrorMessage = "يجب أن تكون كمية المخزون 0 أو أكبر")]
        public int Inventory { get; set; }

        [Required(ErrorMessage = "يرجى اختيار الألوان الخاصة بالمنتج")]
        public List<string> ColorNames { get; set; }

        // Optional properties for URLs
        public string? MainImageUrl { get; set; }
        public List<string>? AdditionalImageUrls { get; set; }
        public List<string>? AdditionalImageUrlsToDelete { get; set; }
        public List<string>? ColorImageUrls { get; set; }
    }
}