using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceCore.Models
{
    public class User : IdentityUser<int>
    {
        [NotMapped]
        public int UserId
        {
            get => base.Id;
            set => base.Id = value;
        }

        public string? Img { get; set; }
        

        [Required(ErrorMessage = "الاسم الكامل مطلوب.")]
        [StringLength(100, ErrorMessage = "الاسم الكامل يجب ألا يتجاوز 100 حرف.")]
        public override string UserName { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب.")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة.")]
        public override string Email { get; set; }

        [Phone(ErrorMessage = "صيغة رقم الهاتف غير صحيحة.")]
        public override string? PhoneNumber { get; set; }

        [StringLength(100, ErrorMessage = "اسم المدينة يجب ألا يتجاوز 100 حرف.")]
        public string? City { get; set; }

        [StringLength(100, ErrorMessage = "اسم المنطقة يجب ألا يتجاوز 100 حرف.")]
        public string? Area { get; set; }

        [StringLength(100, ErrorMessage = "اسم الشارع يجب ألا يتجاوز 100 حرف.")]
        public string? Street { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? EmailConfirmationExpiry { get; set; }

        public string Role { get; set; } 
        public Cart Cart { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}