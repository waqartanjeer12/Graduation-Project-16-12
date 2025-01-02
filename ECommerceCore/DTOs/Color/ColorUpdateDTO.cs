using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.DTOs.Color
{
    public class ColorUpdateDTO
    {

        public string? Name { get; set; }

        public IFormFile? ColorImage { get; set; }
    }
}
