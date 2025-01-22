using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.DTOs.User
{
    public class GetUsersForAdminDTO
    {
        public int UserId { get; set; }
        public string ImgUrl { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }

        public bool IsActive { get; set; }
        public DateTime createdAt { get; set; }
    }
}
