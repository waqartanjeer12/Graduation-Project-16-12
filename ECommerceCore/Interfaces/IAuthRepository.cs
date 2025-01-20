using ECommerceCore.DTOs.Account;
using System.Threading.Tasks;

namespace ECommerceCore.Interfaces
{
    public interface IAuthRepository
    {
        Task<string> RegisterAsync(RegisterDTO registerDTO);
        Task<string> ConfirmEmailAsync(string email, string token);
        Task<string> LoginAsync(LoginDTO loginDTO);
        Task<string> SendEmailConfirmationLink(ForgotPassword forgotPassword);
        Task<string> ResetPasswordAsync(ResetPasswordDTO resetPassword);
    }
}