using ECommerceCore.DTOs.Cart;
using ECommerceCore.Models;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceInfrastructure.Repositories
{
    public interface ICartRepository
    {
        Task<Dictionary<string, string[]>> AddItemToCartAsync(CartAddItemsToCartDTO createDto, ClaimsPrincipal userClaims);
        Task<List<CartGetAllItemsDTO>> GetAllCartItemsAsync(ClaimsPrincipal userClaims);
        Task<Dictionary<string, string[]>> ClearCartItemsByItemIdsAsync(ClaimsPrincipal userClaims, int[] itemIds);
        Task<Dictionary<string, string[]>> RemoveItemFromCartAsync(ClaimsPrincipal userClaims, int cartItemId);
        Task<Dictionary<string, string[]>> IncreaseQuantityAsync(ClaimsPrincipal userClaims, int itemId);
        Task<Dictionary<string, string[]>> DecreaseQuantityAsync(ClaimsPrincipal userClaims, int itemId);
    }
}