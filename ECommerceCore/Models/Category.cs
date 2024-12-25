using System.ComponentModel.DataAnnotations;

namespace ECommerceCore.Models
{
    public class Category
    {
        [Key]

        public int Id { get; set; }

        [Required(ErrorMessage = "يرجى تحميل صورة للفئة")]
        public string Img { get; set; }

        [Required(ErrorMessage = "يرجى ادخال اسم للفئة")]
        [MaxLength(100, ErrorMessage = "يرجى ادخال اسم لا يتجاوز 100 حرف")]
        public string Name { get; set; }

        // Navigation property for related products
        public ICollection<Product> Products { get; set; }
    }

  
}
