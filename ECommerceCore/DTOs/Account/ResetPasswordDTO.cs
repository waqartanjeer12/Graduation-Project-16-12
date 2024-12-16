using System.ComponentModel.DataAnnotations;
using ECommerceCore.DTOs.Account;
public class ResetPasswordDTO
{
    [Required(ErrorMessage = "المعرف مطلوب.")]
    public string userId { get; set; }

    [Required(ErrorMessage = "رمز إعادة تعيين كلمة المرور مطلوب.")]
    public string token { get; set; }

    [Required(ErrorMessage = "كلمة المرور الجديدة مطلوبة.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "يجب أن تتراوح كلمة المرور بين 6 و100 حرف.")]
    public string newPassword { get; set; }
}