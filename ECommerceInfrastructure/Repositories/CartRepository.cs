﻿using ECommerceCore.DTOs.Cart;
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
                return null;
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync();

            if (cart == null)
            {
                _logger.LogWarning("There is no cart for this user.");
                return null;
            }

            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == addItemDto.ProductId && ci.ColorName == addItemDto.ColorName);

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
                var existingProductItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == addItemDto.ProductId);

                if (existingProductItem != null)
                {
                    // Product exists but with a different color, add as a new item
                    cartItem = new CartItem
                    {
                        ProductId = addItemDto.ProductId,
                        Quantity = addItemDto.Quantity,
                        Cart = cart,
                        ColorName = addItemDto.ColorName
                    };
                    cart.CartItems.Add(cartItem);
                    updatedQuantity = addItemDto.Quantity;
                }
                else
                {
                    // Product does not exist in the cart, add as a new item
                    cartItem = new CartItem
                    {
                        ProductId = addItemDto.ProductId,
                        Quantity = addItemDto.Quantity,
                        Cart = cart,
                        ColorName = addItemDto.ColorName
                    };
                    cart.CartItems.Add(cartItem);
                    updatedQuantity = addItemDto.Quantity;
                }
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
                ItemId = cartItem.CartItemId,  // Use CartItemId instead of CartId
                products = cartReadProducts,
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
                .ToListAsync();

            // List to store the result
            var result = new List<CartGetAllItemsDTO>();

            // Iterate through each cart
            foreach (var cart in carts)
            {
                // Map CartItems to CartReadAddItemsToCartDTO
                var items = new List<CartReadAddItemsToCartDTO>();

                foreach (var ci in cart.CartItems)
                {
                    // Fetch color details based on color name
                    var color = await _context.Colors.FirstOrDefaultAsync(c => c.Name == ci.ColorName);

                    var cartReadProducts = new CartReadProducts
                    {
                        ProductId = ci.ProductId,
                        Name = ci.Product.Name,
                        Description = ci.Product.Description,
                        MainImageUrl = ci.Product.MainImage,
                        Price = ci.Product.Price,
                        OriginalPrice = ci.Product.OriginalPrice,
                        ColorDetails =  new ColorReadDTO
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

                // Calculate the total price and total original price
                var totalPrice = items.Sum(item => item.products.Price * item.Quantity);
                var totalOriginalPrice = items.Sum(item => item.products.OriginalPrice.HasValue ? item.products.OriginalPrice.Value * item.Quantity : 0);

                // Add the cart details including the list of items and total prices to the result list
                result.Add(new CartGetAllItemsDTO
                {
                    CartId = cart.Id,
                    items = items, // This will be empty if there are no items in the cart
                    totalPrice = totalPrice, // This will be 0 if there are no items
                    totalOriginalPrice = totalOriginalPrice // This will be 0 if there are no items
                });
            }

            return result;
        }
        public async Task<bool> ClearCartItemsByItemIdsAsync(int cartId, int[] itemIds)
        {
            _logger.LogInformation($"Fetching cart with ID: {cartId} and clearing items with specified item IDs.");

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

            // Filter the cart items to be removed based on the specified item IDs
            var itemsToRemove = cart.CartItems
                .Where(ci => itemIds.Contains(ci.CartItemId)) // 
                .ToList();

            if (itemsToRemove.Count == 0)
            {
                _logger.LogWarning($"No items found in cart with ID: {cartId} matching the specified item IDs.");
                return false;
            }

            // Remove the filtered cart items
            _logger.LogInformation($"Removing {itemsToRemove.Count} items from cart with ID: {cartId}.");
            _context.CartItems.RemoveRange(itemsToRemove);

            // Save changes to the database
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Successfully cleared specified items from cart with ID: {cartId}.");
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