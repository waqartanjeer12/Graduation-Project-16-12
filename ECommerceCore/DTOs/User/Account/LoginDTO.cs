using System.ComponentModel.DataAnnotations;

namespace ECommerceCore.DTOs.User.Account
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "البريد الإلكتروني مطلوب.")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "كلمة المرور مطلوبة.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "يجب أن تتراوح كلمة المرور بين 6 و100 حرف.")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}