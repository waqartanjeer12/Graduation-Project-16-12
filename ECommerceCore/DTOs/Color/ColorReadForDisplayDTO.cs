using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.DTOs.Color
{
    public class ColorReadForDisplayDTO
    {
        [Required(ErrorMessage = "يرجى إدخال اللون")]
        public string ColorImage { get; set; }
    }
}
