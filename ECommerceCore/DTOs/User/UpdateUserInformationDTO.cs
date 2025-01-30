using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.DTOs.User
{
    public class UpdateUserInformationDTO
    {
        public IFormFile? Img { get; set; }
        [Required(ErrorMessage = "يرجى إدخال الاسم الكامل")]

        public string? Name { get; set; }
        public string? City { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Area { get; set; }

        public string? Street { get; set; }

        public string? ImgUrl { get; set; }  
    }
}
