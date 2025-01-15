using ECommerceCore.DTOs.Cart;
using ECommerceCore.Models;

namespace ECommerceInfrastructure.Repositories
{
    public interface ICartRepository
    {
        Task<CartReadAddItemsToCartDTO> AddItemToCartAsync(CartAddItemsToCartDTO addItemDto);
        Task <List<CartGetAllItemsDTO>> GetAllCartItemsAsync();
        Task<bool> ClearCartAsync(int cartId);
        Task<bool> ClearCartItemsByItemIdsAsync(int cartId, int[] itemIds);
        Task<bool> RemoveItemFromCartAsync(int cartItemId);
        Task<bool> IncreaseQuantityAsync(int itemId);
        Task<bool> DecreaseQuantityAsync(int itemId);

    }
}