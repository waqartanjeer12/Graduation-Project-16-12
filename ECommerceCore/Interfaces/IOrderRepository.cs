using ECommerceCore.DTOs.Order;
using System.Security.Claims;

namespace ECommerceInfrastructure.Repositories
{
    public interface IOrderRepository
    {
        Task<Dictionary<string, string[]>> CreateOrderAsync(CreateOrderDTO createOrderDTO, ClaimsPrincipal userClaims);
        Task<List<GetAllOrdersForAdmin>> GetAllOrdersForAdminAsync();
        Task<ReadOrderDetail> GetOrderDetailsByIdAsync(int orderId);
        Task<Dictionary<string, string[]>> UpdateOrderStatusByUserAsync(ClaimsPrincipal userClaims, UpdateOrderStatusDTO updateOrderStatusDTO);
        Task<Dictionary<string, string[]>> UpdateOrderStatusByIdAsync(int orderId, UpdateOrderStatusDTO updateOrderStatusDTO);
    }
}