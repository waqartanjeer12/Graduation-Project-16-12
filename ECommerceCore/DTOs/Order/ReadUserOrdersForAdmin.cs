using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.DTOs.Order
{
    public class ReadUserOrdersForAdmin
    {
        public int OrderId { get; set; }

        [Required(ErrorMessage = " رقم الطلب مطلوب")]
        public int OrderNumber { get; set; }
        [Required(ErrorMessage = "يرجى ارجاع اسم المستخدم")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "يرجى ارحاع تاريخ الطلب ")]
        public String orderDate { get; set; }
        [Required(ErrorMessage = "يرجى ارجاع حالة الطلب")]
        public String orderStatus { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "يجب أن يكون السعر قيمة موجبة")]
        [Precision(18, 2)]
        [Required(ErrorMessage = "  يرجى ارجاع السعر الكلي للطلب")]
        public decimal orderTotalPrice { get; set; }
    }
}