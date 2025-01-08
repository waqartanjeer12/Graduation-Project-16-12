using ECommerceCore.DTOs.Cart;
using ECommerceCore.Models;

namespace ECommerceInfrastructure.Repositories
{
    public interface ICartRepository
    {
        Task<CartReadAddItemsToCartDTO> AddItemToCartAsync(CartAddItemsToCartDTO addItemDto);
        Task <List<CartGetAllItemsDTO>> GetAllCartItemsAsync();
        Task<bool> ClearCartAsync(int cartId);
        Task<bool> RemoveItemFromCartAsync(int productId);


    }
}