using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ECommerceCore.DTOs.Order
{
    public class ReadCreateOrderDTO
    {
        [Required(ErrorMessage = " رقم الطلب مطلوب")]
        public int OrderId { get; set; } // Updated property name to OrderId
        [Required(ErrorMessage = "يرجى ادخال الاسم الأول")]
        public string FName { get; set; }
        [Required(ErrorMessage = "يرجى ادخال الاسم الثاني")]
        public string LName { get; set; }
        [Required(ErrorMessage = "يرجى ادخال رقم الهاتف")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "يرجى ادخال المدينة")]
        public string City { get; set; }
        [Required(ErrorMessage = "يرجى ادخال الشارع")]
        public string Street { get; set; }
        [Required(ErrorMessage = "يرجى ادخال المنطقة")]
        public string Area { get; set; }
        [Required(ErrorMessage = "السعر الكلي قبل الدفع مطلوب")]
        [Range(0, double.MaxValue, ErrorMessage = "يجب أن يكون السعر قيمة موجبة")]
        [Precision(18, 2)]
        public decimal TotalPriceBeforeShipping { get; set; }
        [Required(ErrorMessage = "سعر الشحن مطلوب")]
        [Range(0, double.MaxValue, ErrorMessage = "يجب أن يكون السعر قيمة موجبة")]
        [Precision(18, 2)]
        public decimal ShippingPrice { get; set; } = 0;
        [Required(ErrorMessage = "السعر الكلي مع سعر الشحن مطلوب")]
        [Range(0, double.MaxValue, ErrorMessage = "يجب أن يكون السعر قيمة موجبة")]
        [Precision(18, 2)]
        public decimal TotalPrice { get; set; }
        [Required(ErrorMessage = "عناصر السلة مطلوبة")]
        public List<ItemsInCart> ItemsInCart { get; set; }
    }
}