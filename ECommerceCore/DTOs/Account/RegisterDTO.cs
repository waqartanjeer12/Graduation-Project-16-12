using System.ComponentModel.DataAnnotations;

public class RegisterDTO
{
    [Required(ErrorMessage = "الاسم الكامل مطلوب.")]
     [StringLength(100, ErrorMessage = "الاسم الكامل يجب ألا يتجاوز 100 حرف.")]
    public string Name { get; set; }  // Changed to PascalCase

    [Required(ErrorMessage = "البريد الإلكتروني مطلوب.")]
    [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة.")]
    public string Email { get; set; }  // Changed to PascalCase

    [Required(ErrorMessage = "كلمة المرور مطلوبة.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "يجب أن تتراوح كلمة المرور بين 6 و100 حرف.")]
    public string Password { get; set; }  // Changed to PascalCase

    [Required(ErrorMessage = "تأكيد كلمة المرور مطلوب.")]
    [Compare("Password", ErrorMessage = "كلمة المرور وتأكيد كلمة المرور لا يتطابقان.")]
    public string ConfirmPassword { get; set; }  // This will compare the ConfirmPassword field with Password

}
