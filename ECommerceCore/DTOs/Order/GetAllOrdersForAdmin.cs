using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ECommerceCore.DTOs.Order
{
    public class GetAllOrdersForAdmin
    {
        [Required(ErrorMessage = " رقم الطلب مطلوب")]
        public int OrderId { get; set; } // Updated property name to OrderId
        [Required(ErrorMessage = "يرجى ادخال الاسم الأول")]
        public DateTime orderDate { get; set; }
        public String orderStatus { get; set; }
        public String orderStatusDetails { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "يجب أن يكون السعر قيمة موجبة")]
        [Precision(18, 2)]
        public decimal totalPrice { get; set; }

    }
}

