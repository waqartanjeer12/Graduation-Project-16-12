using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ProductImage
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "رابط الصورة مطلوب")]
    [MaxLength(500, ErrorMessage = "رابط الصورة لا يمكن أن يتجاوز 500 حرف.")]
    public string ImageUrl { get; set; }

    // Foreign key for Product
    [ForeignKey("Product")]
    public int ProductId { get; set; }

    public Product Product { get; set; }
}
