using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using ECommerceCore.Models;
using Microsoft.EntityFrameworkCore;

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "اسم المنتج مطلوب")]
    [MaxLength(200, ErrorMessage = "يجب ألا يزيد اسم المنتج عن 200 حرف.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "وصف المنتج مطلوب")]
    [MaxLength(1000, ErrorMessage = "يجب ألا يزيد وصف المنتج عن 1000 حرف.")]
    public string Description { get; set; }

    [Required(ErrorMessage = "السعر مطلوب")]
    [Range(0, double.MaxValue, ErrorMessage = "يجب أن يكون السعر أكبر من أو يساوي 0.")]
    [Precision(18, 2)] // Specify precision and scale for Price
    public decimal Price { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "يجب أن يكون السعر الأصلي أكبر من أو يساوي 0.")]
    [Precision(18, 2)] // Specify precision and scale for Price
    public decimal? OriginalPrice { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "يجب أن تكون الكمية المخزونة أكبر من أو تساوي 0.")]
    public int Inventory { get; set; }

    // Foreign key for Category
    [ForeignKey("Category")]
    public int CategoryId { get; set; }

    public Category Category { get; set; }

    [Required(ErrorMessage = "الصورة الرئيسية للمنتج مطلوبة")]
    [MaxLength(500, ErrorMessage = "رابط الصورة لا يمكن أن يتجاوز 500 حرف.")]
    public string MainImage { get; set; }

    // Navigation properties
    public ICollection<Product> Products { get; set; }
    public ICollection<ProductImage> AdditionalImages { get; set; } = new List<ProductImage>();
    public ICollection<ProductColor> Colors { get; set; } 
}
