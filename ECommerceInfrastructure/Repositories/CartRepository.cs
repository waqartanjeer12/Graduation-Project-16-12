using ECommerceCore.DTOs.Cart;
using ECommerceCore.DTOs.Color;
using ECommerceCore.Models;
using ECommerceInfrastructure.Configurations.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ECommerceInfrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CartRepository> _logger;
        private readonly UserManager<User> _userManager;

        public CartRepository(ApplicationDbContext context, ILogger<CartRepository> logger, UserManager<User> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<User> GetUserFromClaimsAsync(ClaimsPrincipal userClaims)
        {
            var userId = userClaims.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<CartReadAddItemsToCartDTO> AddItemToCartAsync(CartAddItemsToCartDTO createDto, ClaimsPrincipal userClaims)
        {
            var user = await GetUserFromClaimsAsync(userClaims);
            if (user == null)
                throw new Exception("User not found.");

            var product = await _context.Products
                .Include(p => p.Colors)
                .ThenInclude(pc => pc.Color)
                .FirstOrDefaultAsync(p => p.Id == createDto.ProductId);
            if (product == null)
                throw new Exception("Product not found.");

            var color = product.Colors.FirstOrDefault(c => c.Color.Name == createDto.ColorName);
            if (color == null)
                throw new Exception("The specified color is not available for this product.");

            if (createDto.Quantity > product.Inventory)
                throw new Exception("Requested quantity exceeds available inventory.");

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null)
            {
                cart = new Cart { UserId = user.Id, CartItems = new List<CartItem>() };
                _context.Carts.Add(cart);
            }

            var existingCartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == product.Id && ci.ColorName == createDto.ColorName);
            if (existingCartItem != null)
            {
                existingCartItem.Quantity += createDto.Quantity;
            }
            else
            {
                var newCartItem = new CartItem
                {
                    ProductId = product.Id,
                    Quantity = createDto.Quantity,
                    ColorName = createDto.ColorName,
                    CartId = cart.Id
                };
                cart.CartItems.Add(newCartItem);
            }

            product.Inventory -= createDto.Quantity;
            await _context.SaveChangesAsync();

            var cartItem = cart.CartItems.Last();
            var readCartItem = new CartReadAddItemsToCartDTO
            {
                ItemId = cartItem.CartItemId,
                Quantity = cartItem.Quantity,
                products = new CartReadProducts
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    MainImageUrl = product.MainImage,
                    Price = product.Price,
                    OriginalPrice = product.OriginalPrice,
                    ColorDetails = new ColorReadDTO
                    {
                        Id = color.Color.Id,
                        Name = color.Color.Name,
                        ColorImage = color.Color.Image
                    }
                }
            };

            return readCartItem;
        }

        public async Task<List<CartGetAllItemsDTO>> GetAllCartItemsAsync(ClaimsPrincipal userClaims)
        {
            var currentUser = await GetUserFromClaimsAsync(userClaims);
            if (currentUser == null)
            {
                _logger.LogError("Authenticated user not found.");
                return null;
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == currentUser.Id);

            if (cart == null)
            {
                _logger.LogWarning($"Cart not found for user ID: {currentUser.Id}");
                return new List<CartGetAllItemsDTO>();
            }

            var items = new List<CartReadAddItemsToCartDTO>();

            foreach (var ci in cart.CartItems)
            {
                var color = await _context.Colors.FirstOrDefaultAsync(c => c.Name == ci.ColorName);

                var cartReadProducts = new CartReadProducts
                {
                    ProductId = ci.ProductId,
                    Name = ci.Product.Name,
                    Description = ci.Product.Description,
                    MainImageUrl = ci.Product.MainImage,
                    Price = ci.Product.Price,
                    OriginalPrice = ci.Product.OriginalPrice,
                    ColorDetails = new ColorReadDTO
                    {
                        Id = color.Id,
                        Name = color.Name,
                        ColorImage = color.Image
                    }
                };

                items.Add(new CartReadAddItemsToCartDTO
                {
                    ItemId = ci.CartItemId,
                    products = cartReadProducts,
                    Quantity = ci.Quantity
                });
            }

            var totalPrice = items.Sum(item => item.products.Price * item.Quantity);
            var totalOriginalPrice = items.Sum(item => item.products.OriginalPrice.HasValue ? item.products.OriginalPrice.Value * item.Quantity : 0);

            var result = new CartGetAllItemsDTO
            {
                CartId = cart.Id,
                items = items,
                totalPrice = totalPrice,
                totalOriginalPrice = totalOriginalPrice
            };

            return new List<CartGetAllItemsDTO> { result };
        }

        public async Task<bool> ClearCartItemsByItemIdsAsync(int cartId, int[] itemIds)
        {
            _logger.LogInformation($"Fetching cart with ID: {cartId} and clearing items with specified item IDs.");

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            if (cart == null)
            {
                _logger.LogWarning($"Cart with ID: {cartId} not found.");
                return false;
            }

            var itemsToRemove = cart.CartItems
                .Where(ci => itemIds.Contains(ci.CartItemId))
                .ToList();

            if (itemsToRemove.Count == 0)
            {
                _logger.LogWarning($"No items found in cart with ID: {cartId} matching the specified item IDs.");
                return false;
            }

            _logger.LogInformation($"Removing {itemsToRemove.Count} items from cart with ID: {cartId}.");
            _context.CartItems.RemoveRange(itemsToRemove);

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Successfully cleared specified items from cart with ID: {cartId}.");
            return true;
        }

        public async Task<bool> RemoveItemFromCartAsync(int cartItemId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.CartItems.Any(ci => ci.CartItemId == cartItemId));

            if (cart == null)
            {
                _logger.LogWarning($"Cart containing item with ID: {cartItemId} not found.");
                return false;
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.CartItemId == cartItemId);
            if (cartItem == null)
            {
                _logger.LogWarning($"Cart item with ID: {cartItemId} not found in the cart.");
                return false;
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Successfully removed item with ID: {cartItemId} from the cart.");
            return true;
        }

        public async Task<bool> IncreaseQuantityAsync(int itemId)
        {
            _logger.LogInformation("Increasing quantity for item ID: {ItemId}", itemId);

            var cartItem = await _context.CartItems
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.CartItemId == itemId);
            if (cartItem == null)
            {
                _logger.LogWarning("Item not found for ID: {ItemId}", itemId);
                return false;
            }

            var inventory = cartItem.Product.Inventory;
            if (cartItem.Quantity >= inventory)
            {
                _logger.LogWarning("Not enough stock for item ID: {ItemId}", itemId);
                return false;
            }

            cartItem.Quantity++;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully increased quantity for item ID: {ItemId}", itemId);
            return true;
        }

        public async Task<bool> DecreaseQuantityAsync(int itemId)
        {
            _logger.LogInformation("Decreasing quantity for item ID: {ItemId}", itemId);

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartItemId == itemId);
            if (cartItem == null)
            {
                _logger.LogWarning("Item not found for ID: {ItemId}", itemId);
                return false;
            }

            if (cartItem.Quantity > 1)
            {
                cartItem.Quantity--;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully decreased quantity for item ID: {ItemId}", itemId);
                return true;
            }
            else
            {
                _logger.LogWarning("Cannot decrease quantity below 1 for item ID: {ItemId}", itemId);
                return false;
            }
        }
    }
}