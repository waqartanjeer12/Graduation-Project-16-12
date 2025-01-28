using ECommerceCore.DTOs.Order;
using ECommerceCore.Models;
using ECommerceCore.Interfaces;
using ECommerceInfrastructure.Configurations.Data;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ECommerceInfrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrderRepository> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public OrderRepository(ApplicationDbContext context, ILogger<OrderRepository> logger, UserManager<User> userManager, IConfiguration configuration)
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
        public async Task<Dictionary<string, string[]>> CreateOrderAsync(CreateOrderDTO createOrderDTO, ClaimsPrincipal userClaims)
        {
            var errors = new Dictionary<string, string[]>();
            _logger.LogInformation("Starting order creation process.");

            // Validate input DTO
            if (createOrderDTO == null)
            {
                _logger.LogWarning("Invalid input data for order creation.");
                errors.Add("Order", new[] { "Invalid input data" });
                return errors;
            }

            // Validate user via token
            var user = await GetUserFromClaimsAsync(userClaims);
            if (user == null)
            {
                _logger.LogWarning("Invalid token provided.");
                errors.Add("User", new[] { "User not found or invalid token." });
                return errors;
            }

            // Fetch CartItems by IDs
            var cartItems = await _context.CartItems
                .Where(ci => createOrderDTO.cartItemIds.Contains(ci.CartItemId))
                .Include(ci => ci.Product)
                .ToListAsync();

            if (cartItems == null || !cartItems.Any())
            {
                _logger.LogWarning("No cart items found for the provided IDs.");
                errors.Add("CartItems", new[] { "Cart items not found" });
                return errors;
            }

            // Check inventory
            foreach (var cartItem in cartItems)
            {
                if (cartItem.Quantity > cartItem.Product.Inventory)
                {
                    _logger.LogWarning("Not enough stock for product ID: {ProductId}", cartItem.ProductId);
                    errors.Add("Inventory", new[] { $"Not enough Inventory for product ID: {cartItem.ProductId}" });
                    return errors;
                }
            }

            // Calculate total price before shipping
            var totalPriceBeforeShipping = cartItems.Sum(ci => ci.Product.Price * ci.Quantity);
            _logger.LogInformation("Total price before shipping: {TotalPriceBeforeShipping}", totalPriceBeforeShipping);

            // Create the Order entity
            var order = new Order
            {
                UserId = user.Id,
                orderDate = DateTime.Now,
                orderStatus = "Pending",
                orderStatusDetails = "Order created",
                FName = createOrderDTO.FName,
                LName = createOrderDTO.LName,
                Phone = createOrderDTO.Phone,
                City = createOrderDTO.City,
                Street = createOrderDTO.Street,
                Area = createOrderDTO.Area,
                totalPriceBeforeShipping = totalPriceBeforeShipping,
                shippingPrice = createOrderDTO.ShippingPrice,
                totalPrice = totalPriceBeforeShipping + createOrderDTO.ShippingPrice,
                OrderItems = cartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    OrderItemMainImageUrl = ci.Product.MainImage,
                    OrderItemPrice = ci.Product.Price,

                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Order created successfully with ID: {OrderId}", order.OrderId);

            // Clear the cart items
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Order creation process completed successfully, and cart items cleared.");

            // Return minimal response
            return null; // No errors
        }
        public async Task<List<GetAllOrdersForAdmin>> GetAllOrdersForAdminAsync()
        {
            var orders = await _context.Orders
                .Select(o => new GetAllOrdersForAdmin
                {
                    OrderId = o.OrderId,
                    orderDate = o.orderDate,
                    orderStatus = o.orderStatus,
                    orderStatusDetails = o.orderStatusDetails,
                    totalPrice = o.totalPrice
                })
                .ToListAsync();

            return orders.Any() ? orders : null;
        }
    }
}