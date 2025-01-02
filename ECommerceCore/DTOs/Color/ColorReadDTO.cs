using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.DTOs.Color
{
    public class ColorReadDTO
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "يرجى إدخال اسم اللون")]
        public string Name { get; set; }

        [Required(ErrorMessage = "يرجى تحميل صورة اللون")]
        public string ColorImage { get; set; }
    }
}