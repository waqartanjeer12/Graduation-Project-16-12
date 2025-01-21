using System.Threading.Tasks;
using System.Threading.Tasks;
using ECommerceCore.DTOs.Account;

namespace ECommerceCore.Interfaces
{
    public interface IAuthRepository
    {
        Task<string> RegisterAsync(RegisterDTO registerDTO);
        Task<string> ConfirmEmailAsync(string email, string token);
        Task<string> LoginAsync(LoginDTO loginDTO);
        Task<string> ForgotPasswordAsync(ForgotPasswordDTO forgotPassword);
        Task<string> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO);
    }
}