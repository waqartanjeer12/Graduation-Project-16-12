using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.DTOs.Order
{
   
        public class UpdateOrderStatusDTO
        {
            [Required(ErrorMessage = " يرجى ادخال حالة الطلب")]
            public string OrderStatus { get; set; }

            [Required(ErrorMessage = "يرجى ادخال تفاصيل حالة الطلب")]
            public string OrderStatusDetails { get; set; }
        }
    }
