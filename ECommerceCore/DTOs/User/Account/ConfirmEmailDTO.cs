using System.ComponentModel.DataAnnotations;

namespace ECommerceCore.DTOs.User.Account
{
    public class ConfirmEmailDTO
    {
        [Required(ErrorMessage = "البريد الإلكتروني مطلوب.")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة.")]
        public string email { get; set; }
        public string token { get; set; }
    }
}