using ECommerceCore.DTOs.Order;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ECommerceInfrastructure.Repositories
{
    public interface IOrderRepository
    {
        Task<Dictionary<string, string[]>> CreateOrderAsync(CreateOrderDTO createOrderDTO, ClaimsPrincipal userClaims);
        
        Task<ReadUserOrders> GetUserOrdersByIdAsync(int orderId);
        Task<ReadOrderDetail> GetOrderDetailsByIdAsync(int orderId);
        Task<Dictionary<string, string[]>> UpdateOrderStatusByIdAsync(int orderId, UpdateOrderStatusDTO updateOrderStatusDTO);
        Task<List<ReadUserOrders>> GetUserOrdersByUserIdAsync(int userId);
        Task<List<ReadUserOrdersForAdmin>> GetAllUserOrdersForAdminAsync(); // New method
        Task<ReadOrderDetailsForAdmin> GetOrderDetailsForAdminByIdAsync(int orderId); // New method
    }
}