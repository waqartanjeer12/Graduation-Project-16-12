﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.DTOs.Order
{
    public class ReadCreateOrderDTO
    {
        [Required(ErrorMessage = " رقم الطلب مطلوب")]
        public int OrderItemId { get; set; }
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
        [Required (ErrorMessage ="السعر الكلي قبل الدفع مطلوب")]
        public decimal totalPriceBeforeShipping { get; set; }
        [Required(ErrorMessage = "سعر الشحن مطلوب")]
        public decimal shippingPrice { get; set; } = 0;
        [Required(ErrorMessage = "السعر الكلي مع سعر الشحن مطلوب")]
        public decimal totalPrice { get; set; }
        [Required(ErrorMessage = "عناصر السلة مطلوبة")]
        public List<ItemsInCart> ItemsInCart { get; set; }




    }
}
