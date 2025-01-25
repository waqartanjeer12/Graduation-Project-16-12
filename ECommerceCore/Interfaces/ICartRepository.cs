using ECommerceCore.DTOs.Cart;
using ECommerceCore.Models;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceInfrastructure.Repositories
{
    public interface ICartRepository
    {
        Task<CartReadAddItemsToCartDTO> AddItemToCartAsync(CartAddItemsToCartDTO createDto, ClaimsPrincipal userClaims);
        Task<List<CartGetAllItemsDTO>> GetAllCartItemsAsync(ClaimsPrincipal userClaims);
        Task<bool> ClearCartItemsByItemIdsAsync(int cartId, int[] itemIds);
        Task<bool> RemoveItemFromCartAsync(int cartItemId);
        Task<bool> IncreaseQuantityAsync(int itemId);
        Task<bool> DecreaseQuantityAsync(int itemId);
    }
}