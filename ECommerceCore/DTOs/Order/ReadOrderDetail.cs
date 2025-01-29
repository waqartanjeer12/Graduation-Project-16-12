using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.DTOs.Order
{
    public class ReadOrderDetail
    {

        [Required(ErrorMessage = " رقم الطلب مطلوب")]
        public int OrderId { get; set; } // Updated property name to OrderId
        [Required(ErrorMessage = "يرجى ادخال الاسم الأول")]
        public DateTime orderDate { get; set; }
        public String orderStatus { get; set; }
        public String orderStatusDetails { get; set; }
        [Required(ErrorMessage = "يرجى ارجاع المنتجات")]
        public List<OrderProducts> OrderProducts { get; set; }
        [Required(ErrorMessage = "  يرجى ارجاع المجموع")]
        [Range(0, double.MaxValue, ErrorMessage = "يجب أن يكون السعر قيمة موجبة")]
        [Precision(18, 2)]
        public decimal totalPriceBeforeShipping { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "يجب أن يكون السعر قيمة موجبة")]
        [Precision(18, 2)]
        [Required(ErrorMessage = "يرجى ادخال رسوم الشحن")]
        public decimal shippingPrice { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "يجب أن يكون السعر قيمة موجبة")]
        [Precision(18, 2)]
        [Required(ErrorMessage = "يرجى ارجاع الاجمالي الكلي")]
        public decimal totalPrice { get; set; }
        [Required(ErrorMessage = "يرجى ارحاع الاسم الكلي")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "يرجى ارجاع رقم الهاتف")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "يرجى ارجاع المدينة")]
        public string City { get; set; }
        [Required(ErrorMessage = "يرجى ارجاع الشارع")]
        public string Street { get; set; }
        [Required(ErrorMessage = "يرجى ارجاع المنطقة")]
        public string Area { get; set; }
    }
}
