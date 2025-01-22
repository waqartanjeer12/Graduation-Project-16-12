using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.DTOs.User
{
    public class GetUserForUserDTO
    {
        [Required(ErrorMessage = "معرف المستخدم مطلوب.")]
        public int UserId { get; set; }
        [Required(ErrorMessage = "الاسم الكامل مطلوب.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "البريد الإلكتروني مطلوب.")]
        public string Email { get; set; }
        public string ImgUrl { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string Area { get; set; }
        public string Street { get; set; }
        

    }
}
