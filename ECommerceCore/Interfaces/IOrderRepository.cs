using ECommerceCore.DTOs.Order;

namespace ECommerceInfrastructure.Repositories
{
    public interface IOrderRepository
    {
        Task<ReadCreateOrderDTO> CreateOrderAsync(CreateOrderDTO createOrderDTO);
    }
}