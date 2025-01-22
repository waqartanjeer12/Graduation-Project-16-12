using System.ComponentModel.DataAnnotations;

namespace ECommerceCore.DTOs.User.Account
{
    public class createUserDTO
    {
        [Required(ErrorMessage = "الاسم الكامل مطلوب.")]
        [StringLength(100, ErrorMessage = "الاسم الكامل يجب ألا يتجاوز 100 حرف.")]

        public string name { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب.")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة.")]
        public string email { get; set; }

        [Required(ErrorMessage = "كلمة المرور مطلوبة.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "يجب أن تتراوح كلمة المرور بين 6 و100 حرف.")]
        public string password { get; set; }

        [Required(ErrorMessage = "تأكيد كلمة المرور مطلوب.")]
        [Compare("password", ErrorMessage = "كلمة المرور وتأكيد كلمة المرور لا يتطابقان.")]
        public string confirmPassword { get; set; }
    }
}
