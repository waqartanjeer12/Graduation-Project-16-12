using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.DTOs.Color
{
    public class ColorCreateDTO
    {
        [Required(ErrorMessage = "يرجى ادخال اسم اللون")]
        [MaxLength(100, ErrorMessage = "يجب ألا يزيد اسم اللون عن 100 حرف.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "يرجى تحميل صورة اللون")]
        public IFormFile ColorImage { get; set; }
    }
}