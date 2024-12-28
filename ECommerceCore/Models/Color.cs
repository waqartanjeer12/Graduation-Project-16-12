using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using ECommerceCore.Models;
using Microsoft.EntityFrameworkCore;

public class Color
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "اسم اللون مطلوب")]
    [MaxLength(100, ErrorMessage = "يجب ألا يزيد اسم اللون عن 100 حرف.")]
    
    public string Name { get; set; }

    [Required(ErrorMessage = "رابط صورة اللون مطلوب")]
    [MaxLength(500, ErrorMessage = "رابط الصورة لا يمكن أن يتجاوز 500 حرف.")]
    public string Image { get; set; }

    // Navigation property
    
    public ICollection<ProductColor> ProductColors { get; set; }
}
