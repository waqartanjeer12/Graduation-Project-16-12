using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.DTOs.User.Account
{
    public class ChangePasswordDTO
    {
        [Required(ErrorMessage = "يرجى إدخال كلمة المرور القديمة")]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "يرجى إدخال كلمة المرور الجديدة")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "يرجى تأكيد كلمة المرور الجديدة")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
