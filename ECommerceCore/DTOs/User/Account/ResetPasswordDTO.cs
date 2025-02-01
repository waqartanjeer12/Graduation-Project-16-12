using System.ComponentModel.DataAnnotations;

namespace ECommerceCore.DTOs.User.Account
{
    public class ResetPasswordDTO
    {
        [Required]
        public string Email { get; set; }
        [Required(ErrorMessage = "يرجى إدخال كود التحقق المرسل إلى بريدك الإلكتروني.")]
        public string Code { get; set; }

        [Required(ErrorMessage = "يرجى إدخال كلمة المرور الجديدة.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "يجب أن تتراوح كلمة المرور بين 6 و100 حرف.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "يرجى تأكيد كلمة المرور.")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "كلمة المرور وتأكيدها غير متطابقين.")]
        public string ConfirmPassword { get; set; }
    }
}