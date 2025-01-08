using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.DTOs.Category
{
    public class CategoryReadDTO
    {
        [Required(ErrorMessage = "يرجى تحميل صورة للفئة")]
        public string Img { get; set; }

        [Required(ErrorMessage = "يرجى ادخال اسم للفئة")]
        [MaxLength(100, ErrorMessage = "يرجى ادخال اسم لا يتجاوز 100 حرف")]

        public string Name { get; set; }


    }
}
