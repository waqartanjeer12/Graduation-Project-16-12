using ECommerceCore.DTOs.Order;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ECommerceInfrastructure.Repositories
{
    public interface IOrderRepository
    {
        Task<Dictionary<string, string[]>> CreateOrderAsync(CreateOrderDTO createOrderDTO, ClaimsPrincipal userClaims);


        Task<ReadOrderDetail> GetOrderDetailsByIdAsync(int orderId);
        Task<Dictionary<string, string[]>> UpdateOrderStatusByIdAsync(int orderId, UpdateOrderStatusDTO updateOrderStatusDTO);
        Task<List<ReadUserOrders>> GetUserOrdersByEmailAsync(string email);
        Task<List<ReadUserOrdersForAdmin>> GetAllUserOrdersForAdminAsync(); // New method
        Task<ReadOrderDetailsForAdmin> GetOrderDetailsForAdminByIdAsync(int orderId); // New method

        Task<Dictionary<string, string[]>> DeleteOrderByIdAsync(int orderId); // New method

    }
}