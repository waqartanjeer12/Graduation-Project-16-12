﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.DTOs.User.Account
{

    public class VerifyResetCodeDTO
    {
        public string Email { get; set; }
        public string Code { get; set; }
    }
}
