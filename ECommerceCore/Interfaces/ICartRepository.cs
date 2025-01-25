using ECommerceCore.DTOs.Cart;
using ECommerceCore.Models;

namespace ECommerceInfrastructure.Repositories
{
    public interface ICartRepository
    {
        Task<CartReadAddItemsToCartDTO> AddItemToCartAsync(CartAddItemsToCartDTO addItemDto);
        Task <List<CartGetAllItemsDTO>> GetAllCartItemsAsync(string token);
        
       Task<bool> ClearCartItemsByItemIdsAsync(int cartId, int[] itemIds);
        Task<bool> RemoveItemFromCartAsync(int cartItemId);
        Task<bool> IncreaseQuantityAsync(int itemId);
        Task<bool> DecreaseQuantityAsync(int itemId);

    }
}