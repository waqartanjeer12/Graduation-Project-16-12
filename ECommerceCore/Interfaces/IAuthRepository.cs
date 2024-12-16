using ECommerceCore.DTOs.Account;
using ECommerceCore.Models;
using System.Threading.Tasks;

namespace ECommerceCore.Interfaces
{
    public interface IAuthRepository
    {
        Task<string> RegisterAsync(User user, string password);
        Task<string> LoginAsync(string email, string password);
        Task<string> ConfirmEmailAsync(string email, string token);
    }
}
