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
        [Required(ErrorMessage = "الصورة مطلوبة")]
        public string Img { get; set; }

        [Required(ErrorMessage = "اسم الفئة مطلوب")]
        [MaxLength(100, ErrorMessage = "يجب ألا يزيد اسم الفئة عن 100 حرف.")]
        public string Name { get; set; }


    }
}
