using System.ComponentModel.DataAnnotations;

public class RegisterDTO
{
    [Required(ErrorMessage = "الاسم الكامل مطلوب.")]
    [StringLength(100, ErrorMessage = "الاسم الكامل يجب ألا يتجاوز 100 حرف.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "البريد الإلكتروني مطلوب.")]
    [MinLength(5, ErrorMessage = "البريد الإلكتروني يجب أن يحتوي على على الأقل 5 حروف.")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [Required(ErrorMessage = "كلمة المرور مطلوبة.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "يجب أن تتراوح كلمة المرور بين 6 و100 حرف.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "تأكيد كلمة المرور مطلوب.")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "كلمة المرور وتأكيد كلمة المرور لا يتطابقان.")]
    public string ConfirmPassword { get; set; }
}