using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.DTOs.Order
{
    public class CreateOrderDTO
    { 

        [Required(ErrorMessage ="يرجى ادخال الاسم الأول")]
        public string FName { get; set; }
        [Required(ErrorMessage ="يرجى ادخال الاسم الثاني")]
        public string LName { get; set; }
        [Required(ErrorMessage = "يرجى ادخال رقم الهاتف")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "يرجى ادخال المدينة")]
        public string City { get; set; }
        [Required(ErrorMessage = "يرجى ادخال الشارع")]
        public string Street { get; set; }
        [Required(ErrorMessage = "يرجى ادخال المنطقة")]
        public string Area { get; set; }
        [Required(ErrorMessage = "يرجى ادخال سعر الشحن")]
        [Range(0, double.MaxValue, ErrorMessage = "يجب أن يكون السعر قيمة موجبة")]
        [Precision(18, 2)]
        public decimal ShippingPrice { get; set; } = 0;
        [Required(ErrorMessage ="يرجى ادخال أرقام ")]
        public int[] cartItemIds { get; set; }
        [Required(ErrorMessage = "يرجى ادخال التوكين الخاص باليوزر")]
        public string token { get; set; }
    }
}
