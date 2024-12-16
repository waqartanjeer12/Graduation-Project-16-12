using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerceCore.Models
{
  
        public class User : IdentityUser<int>

    {
       
        [Url(ErrorMessage = "رابط الصورة غير صالح.")]
            
            public string? img { get; set; }

            [Required(ErrorMessage = "الاسم الكامل مطلوب.")]
            [StringLength(100, ErrorMessage = "الاسم الكامل يجب ألا يتجاوز 100 حرف.")]
           
            public override string UserName { get; set; }

            [Required(ErrorMessage = "البريد الإلكتروني مطلوب.")]
            [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة.")]
            public override string Email { get; set; }

            
            [Phone(ErrorMessage = "صيغة رقم الهاتف غير صحيحة.")]
            public override string? PhoneNumber { get; set; }

           
            [StringLength(200, ErrorMessage = "العنوان يجب ألا يتجاوز 200 حرف.")]
            public string? address { get; set; }

            public bool IsActive { get; set; } = true;

           
            public DateTime createdAt { get; set; } = DateTime.UtcNow;
        public DateTime? EmailConfirmationExpiry { get; set; }
    }
    }


