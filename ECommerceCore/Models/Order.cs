﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public DateTime orderDate { get; set; }
        public String orderStatus { get; set; }
        public String orderStatusDetails { get; set; }
        [Required(ErrorMessage = "يرجى ادخال الاسم الأول")]
        public string FName { get; set; }
        [Required(ErrorMessage = "يرجى ادخال الاسم الثاني")]
        public string LName { get; set; }
        [Required(ErrorMessage = "يرجى ادخال رقم الهاتف")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "يرجى ادخال اسم المدينة")]
        public string City { get; set; }
        [Required(ErrorMessage = "يرجى ادخال اسم الشارع")]
        public string Street { get; set; }
        [Required(ErrorMessage = "يرجى ادخال اسم المنطقة")]
        public string Area { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "يجب أن يكون السعر قيمة موجبة")]
        [Precision(18, 2)]
        public decimal totalPriceBeforeShipping { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "يجب أن يكون السعر قيمة موجبة")]
        [Precision(18, 2)]
        public decimal shippingPrice { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "يجب أن يكون السعر قيمة موجبة")]
        [Precision(18, 2)]
        public decimal totalPrice { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
       

        public ICollection<CartItem> CartItems { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
