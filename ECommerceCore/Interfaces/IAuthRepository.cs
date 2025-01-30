using System.Security.Claims;
using System.Threading.Tasks;
using System.Threading.Tasks;
using ECommerceCore.DTOs.User.Account;

namespace ECommerceCore.Interfaces
{
    public interface IAuthRepository
    {
        Task<Dictionary<string, string[]>> RegisterAsync(RegisterDTO registerDTO);
        Task<Dictionary<string, string[]>> ConfirmEmailAsync(string email, string token);

        Task<string> LoginAsync(LoginDTO loginDTO);
        Task<string> SendResetCodeAsync(string email);
        Task<string> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO,string token);
        Task<Dictionary<string, string[]>> ChangePasswordAsync(ClaimsPrincipal userClaims, ChangePasswordDTO changePasswordDTO);
    }
}