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

    
    [MaxLength(1000, ErrorMessage = "يجب ألا يزيد وصف المنتج عن 1000 حرف.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "يرجى إدخال السعر الخاص بالمنتج")]
    [Range(0.01, double.MaxValue, ErrorMessage = "يجب أن يكون السعر قيمة موجبة")]
    [Precision(18, 2)] // Specify precision and scale for Price

    public decimal Price { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "يرجى إدخال سعر لا يقل عن 0")]
    [Precision(18, 2)] // Specify precision and scale for Price
    public decimal? OriginalPrice { get; set; }

    [Required(ErrorMessage = "يرجى تحديد كمية المخزون المتاحة للمنتج")]
    [Range(0, int.MaxValue, ErrorMessage = "يجب أن تكون كمية المخزون 0 أو أكبر")]
    public int Inventory { get; set; }

    // Foreign key for Category
    [ForeignKey("Category")]
    public int CategoryId { get; set; }

    public Category Category { get; set; }

    [Required(ErrorMessage = "يرجى تحميل الصورة الرئيسية للمنتج")]
    public string MainImage { get; set; }

    // Navigation properties
   
    public ICollection<ProductImage> AdditionalImages { get; set; } 
    public ICollection<ProductColor> Colors { get; set; }
    public ICollection<CartItem> CartItem { get; set; }
}
