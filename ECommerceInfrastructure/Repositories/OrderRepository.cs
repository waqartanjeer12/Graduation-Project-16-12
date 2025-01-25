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
            _configuration = configuration;
        }

        private async Task<User> GetUserByTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JWT:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["JWT:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

                return await _userManager.FindByIdAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Token validation failed: {ex.Message}");
                return null;
            }
        }

        public async Task<ReadCreateOrderDTO> CreateOrderAsync(CreateOrderDTO createOrderDTO)
        {
            _logger.LogInformation("Starting order creation process.");

            // Validate input DTO
            if (createOrderDTO == null)
            {
                _logger.LogWarning("Invalid input data for order creation.");
                throw new ArgumentException("Invalid input data");
            }

            // Validate user via token
            var user = await GetUserByTokenAsync(createOrderDTO.token);
            if (user == null)
            {
                _logger.LogWarning("Invalid token provided.");
                throw new ArgumentException("User not found or invalid token.");
            }

            // Fetch CartItems by IDs
            var cartItems = await _context.CartItems
                .Where(ci => createOrderDTO.cartItemIds.Contains(ci.CartItemId))
                .Include(ci => ci.Product)
                .ToListAsync();

            if (cartItems == null || !cartItems.Any())
            {
                _logger.LogWarning("No cart items found for the provided IDs.");
                throw new ArgumentException("Cart items not found");
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

            // Fetch the saved order with order items
            var savedOrder = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == order.OrderId);

            // Map OrderItems to ItemsInCart DTO with OrderItemId and CartItemId
            var itemsInCart = savedOrder.OrderItems.Select(oi => new ItemsInCart
            {
                OrderItemId = oi.OrderItemId,
                
                CartMainImageUrl = oi.OrderItemMainImageUrl,
                CartItemPrice = oi.OrderItemPrice,
                Quantity = oi.Quantity
            }).ToList();

            // Create the ReadCreateOrderDTO
            var readCreateOrderDTO = new ReadCreateOrderDTO
            {
                OrderId = savedOrder.OrderId,
                FName = savedOrder.FName,
                LName = savedOrder.LName,
                Phone = savedOrder.Phone,
                City = savedOrder.City,
                Street = savedOrder.Street,
                Area = savedOrder.Area,
                TotalPriceBeforeShipping = savedOrder.totalPriceBeforeShipping,
                ShippingPrice = savedOrder.shippingPrice,
                TotalPrice = savedOrder.totalPrice,
                ItemsInCart = itemsInCart
            };

            _logger.LogInformation("Order creation process completed successfully.");

            return readCreateOrderDTO;
        }
    }
}