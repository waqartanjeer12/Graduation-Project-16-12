using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.DTOs.Category
{
    public class CategoryReadForIdName
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "يرجى تحميل صورة للفئة")]
        
        [MaxLength(100, ErrorMessage = "يرجى ادخال اسم لا يتجاوز 100 حرف")]
        public string Name { get; set; }

    }
}
