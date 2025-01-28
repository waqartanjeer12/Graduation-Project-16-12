using ECommerceCore.DTOs.Order;
using System.Security.Claims;

namespace ECommerceInfrastructure.Repositories
{
    public interface IOrderRepository
    {
        Task<Dictionary<string, string[]>> CreateOrderAsync(CreateOrderDTO createOrderDTO, ClaimsPrincipal userClaims);
        Task<List<GetAllOrdersForAdmin>> GetAllOrdersForAdminAsync();
    }
}