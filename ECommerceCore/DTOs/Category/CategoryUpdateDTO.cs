using Microsoft.AspNetCore.Http;

namespace ECommerceCore.DTOs.Category
{
    public class CategoryUpdateDTO
    {
        public string? ImgUrl { get; set; } // Existing image URL (optional)
        public IFormFile? Img { get; set; } // New image file (optional)
        public string? Name { get; set; }  // Category name (optional)
    }
}
