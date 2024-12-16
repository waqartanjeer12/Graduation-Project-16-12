using System.ComponentModel.DataAnnotations;

namespace ECommerceCore.DTOs.Account
{
    public class loginDTO
    {
        [Required(ErrorMessage = "البريد الإلكتروني مطلوب.")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة.")]
        public string email { get; set; }

        [Required(ErrorMessage = "كلمة المرور مطلوبة.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "يجب أن تتراوح كلمة المرور بين 6 و100 حرف.")]
        public string password { get; set; }

    }
}
