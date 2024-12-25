using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ECommerceCore.DTOs.Category
{
    public class CategoryCreateDTO

    {
        [Required(ErrorMessage = "يرجى تحميل صورة للفئة")]
        public IFormFile Img { get; set; }

        [Required(ErrorMessage = "يرجى ادخال اسم للفئة")]
        [MaxLength(100, ErrorMessage = "يرجى ادخال اسم لا يتجاوز 100 حرف")]
        public string Name { get; set; }


    }
}
