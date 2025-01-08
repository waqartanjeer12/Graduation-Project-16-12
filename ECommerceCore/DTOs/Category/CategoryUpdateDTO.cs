using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ECommerceCore.DTOs.Category
{
    public class CategoryUpdateDTO
    {
        public string? ImgUrl { get; set; } // Existing image URL (optional)
        public IFormFile? Img { get; set; } // New image file (optional)

        [Required(ErrorMessage = "يرجى ادخال اسم للفئة")]
        [MaxLength(100, ErrorMessage = "يرجى ادخال اسم لا يتجاوز 100 حرف")]
        public string Name { get; set; }  // Category name (optional)
    }
}
