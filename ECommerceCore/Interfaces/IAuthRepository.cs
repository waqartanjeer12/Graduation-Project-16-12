using System.Threading.Tasks;
using System.Threading.Tasks;
using ECommerceCore.DTOs.User.Account;

namespace ECommerceCore.Interfaces
{
    public interface IAuthRepository
    {
        Task<Dictionary<string, string[]>> RegisterAsync(RegisterDTO registerDTO);
       
        Task<string> LoginAsync(LoginDTO loginDTO);
        Task<string> ForgotPasswordAsync(ForgotPasswordDTO forgotPassword);
        Task<string> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO);
    }
}