using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ECommerceCore.DTOs.Category
{
    public class CategoryCreateUpdateDTO

    {
        [Required(ErrorMessage = "الصورة مطلوبة")]
        public IFormFile Img { get; set; }

        [Required(ErrorMessage = "اسم الفئة مطلوب")]
        [MaxLength(100, ErrorMessage = "يجب ألا يزيد اسم الفئة عن 100 حرف.")]
        public string Name { get; set; }


    }
}
