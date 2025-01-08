using ECommerceCore.DTOs.Cart;
using ECommerceCore.DTOs.Color;
using ECommerceCore.Models;
using ECommerceInfrastructure.Configurations.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceInfrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CartRepository> _logger;

        public CartRepository(ApplicationDbContext context, ILogger<CartRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CartReadAddItemsToCartDTO> AddItemToCartAsync(CartAddItemsToCartDTO addItemDto)
        {
            if (addItemDto == null)
            {
                _logger.LogError("addItemDto is null.");
                return null;
            }

            // Log the incoming addItemDto details
            _logger.LogInformation($"Received request to add item: ProductId={addItemDto.ProductId}, Quantity={addItemDto.Quantity}, ColorName={addItemDto.ColorName}");

            var product = await _context.Products
                .Include(p => p.Colors)
                .ThenInclude(pc => pc.Color)
                .FirstOrDefaultAsync(p => p.Id == addItemDto.ProductId);

            if (product == null)
            {
                _logger.LogError($"Product with ID {addItemDto.ProductId} not found.");
                return null;
            }

            // Log the retrieved product details
            _logger.LogInformation($"Retrieved product: Id={product.Id}, Name={product.Name}, Inventory={product.Inventory}");

            if (product.Colors == null || !product.Colors.Any())
            {
                _logger.LogError($"No colors found for product ID {addItemDto.ProductId}.");
                return null;
            }

            var selectedColor = product.Colors.FirstOrDefault(c => c.Color != null && c.Color.Name == addItemDto.ColorName);

            if (selectedColor == null)
            {
                _logger.LogError($"Color {addItemDto.ColorName} not found for product ID {addItemDto.ProductId}.");
                return null;
            }

            // Log the selected color details
            _logger.LogInformation($"Selected color: Id={selectedColor.Color.Id}, Name={selectedColor.Color.Name}");

            if (product.Inventory < addItemDto.Quantity)
            {
                _logger.LogWarning($"Insufficient inventory for product {addItemDto.ProductId}. Requested: {addItemDto.Quantity}, Available: {product.Inventory}");
                return new CartReadAddItemsToCartDTO
                {
                    IsInventorySufficient = false
                };
            }

            product.Inventory -= addItemDto.Quantity;  // Decrease inventory

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync();

            if (cart == null)
            {
                cart = new Cart
                {
                    CartItems = new List<CartItem>()
                };
                _context.Carts.Add(cart);
            }

            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == addItemDto.ProductId);

            int updatedQuantity;
            CartItem cartItem;
            if (existingItem != null)
            {
                existingItem.Quantity += addItemDto.Quantity;
                updatedQuantity = existingItem.Quantity;
                cartItem = existingItem;
            }
            else
            {
                cartItem = new CartItem
                {
                    ProductId = addItemDto.ProductId,
                    Quantity = addItemDto.Quantity,
                    Cart = cart
                };
                cart.CartItems.Add(cartItem);
                updatedQuantity = addItemDto.Quantity;
            }

            await _context.SaveChangesAsync();

            var cartReadProducts = new CartReadProducts
            {
                ProductId = product.Id,
                Name = product.Name,
                Description = product.Description,
                MainImageUrl = product.MainImage,
                Price = product.Price,
                OriginalPrice = product.OriginalPrice,
                ColorDetails = new ColorReadDTO
                {
                    Id = selectedColor.Color.Id,
                    Name = selectedColor.Color.Name,
                    ColorImage = selectedColor.Color.Image
                }
                
            };

            var cartReadAddItemsToCartDto = new CartReadAddItemsToCartDTO
            {
                ItemId = cartItem.CatrItemId,  // Use CartItemId instead of CartId
                items = cartReadProducts,
                IsInventorySufficient = true,
                Quantity = updatedQuantity
            };

            return cartReadAddItemsToCartDto;
        }

        public async Task<List<CartGetAllItemsDTO>> GetAllCartItemsAsync()
        {
            // Fetch all carts including related entities
            var carts = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .ThenInclude(p => p.Colors)
                .ThenInclude(pc => pc.Color)
                .ToListAsync();

            // List to store the result
            var result = new List<CartGetAllItemsDTO>();

            // Iterate through each cart
            foreach (var cart in carts)
            {
                // Map CartItems to CartReadAddItemsToCartDTO
                var items = cart.CartItems.Select(ci => {
                    var cartReadProducts = new CartReadProducts
                    {
                        ProductId = ci.ProductId,
                        Name = ci.Product.Name,
                        Description = ci.Product.Description,
                        MainImageUrl = ci.Product.MainImage,
                        Price = ci.Product.Price,
                        OriginalPrice = ci.Product.OriginalPrice
                    };

                    // Set ColorDetails if available
                    if (ci.Product.Colors != null && ci.Product.Colors.Any())
                    {
                        var firstColor = ci.Product.Colors.First().Color;
                        cartReadProducts.ColorDetails = new ColorReadDTO
                        {
                            Id = firstColor.Id,
                            Name = firstColor.Name,
                            ColorImage = firstColor.Image
                        };
                    }
                    else
                    {
                        cartReadProducts.ColorDetails = null;
                    }

                    return new CartReadAddItemsToCartDTO
                    {
                        ItemId = ci.CatrItemId,
                        items = cartReadProducts,
                        IsInventorySufficient = ci.Product.Inventory >= ci.Quantity,
                        Quantity = ci.Quantity // Set the quantity here
                    };
                }).ToList();

                // Calculate the total price and total original price
                var totalPrice = items.Sum(item => item.items.Price * item.Quantity);
                var totalOriginalPrice = items.Sum(item => item.items.OriginalPrice.HasValue ? item.items.OriginalPrice.Value * item.Quantity : 0);

                // Add the cart details including the list of items and total prices to the result list
                result.Add(new CartGetAllItemsDTO
                {
                    CartId = cart.Id,
                    items = items,
                    totalPrice = totalPrice,
                    totalOriginalPrice = totalOriginalPrice
                });
            }

            return result;
        }
       

        public async Task<bool> ClearCartAsync(int cartId)
        {
            _logger.LogInformation($"Fetching cart with ID: {cartId} to clear items.");

            // Fetch the cart with the specified cartId
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            // If the cart is not found, log and return false
            if (cart == null)
            {
                _logger.LogWarning($"Cart with ID: {cartId} not found.");
                return false;
            }

            // Remove all cart items
            _logger.LogInformation($"Removing {cart.CartItems.Count} items from cart with ID: {cartId}.");
            _context.CartItems.RemoveRange(cart.CartItems);

            // Save changes to the database
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Successfully cleared cart with ID: {cartId}.");
            return true;
        }

        public async Task<bool> RemoveItemFromCartAsync(int productId)
        {
            // Fetch the cart that contains the item with the specified productId
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.CartItems.Any(ci => ci.ProductId == productId));

            // If the cart or item is not found, return false
            if (cart == null)
            {
                _logger.LogWarning($"Cart containing product with ID: {productId} not found.");
                return false;
            }

            // Find the cart item to be removed
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem == null)
            {
                _logger.LogWarning($"Cart item with product ID: {productId} not found in the cart.");
                return false;
            }

            // Remove the cart item
            _context.CartItems.Remove(cartItem);

            // Save changes to the database
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Successfully removed item with product ID: {productId} from the cart.");
            return true;
        }
    }
}