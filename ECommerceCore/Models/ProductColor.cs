using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.Models
{
    public class ProductColor
    {
      
        public int ProductId { get; set; }
        public Product Product { get; set; }

        
        public int ColorId { get; set; }
        public Color Color { get; set; }
    }

}
