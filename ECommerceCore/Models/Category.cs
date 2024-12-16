using System.ComponentModel.DataAnnotations;

namespace ECommerceCore.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "رابط الصورة مطلوب")]
        public string Img { get; set; } 

        [Required(ErrorMessage = "اسم الفئة مطلوب")]
        [MaxLength(100, ErrorMessage = "يجب ألا يزيد اسم الفئة عن 100 حرف.")]
        public string Name { get; set; }

      
    }
}
