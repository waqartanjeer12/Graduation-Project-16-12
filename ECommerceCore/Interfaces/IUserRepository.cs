using ECommerceCore.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.Interfaces
{
    public interface IUserRepository
    {
        Task<GetUserForUserDTO> GetUserByEmailAsync(String email);
        Task <List<GetUsersForAdminDTO>> GetAllUsersAdminAsync();
        Task<Dictionary<string, string[]>> UpdateUserAsync(ClaimsPrincipal userClaims, UpdateUserInformationDTO dto);
    }
}
