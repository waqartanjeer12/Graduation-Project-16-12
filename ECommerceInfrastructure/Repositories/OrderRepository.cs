using ECommerceCore.DTOs.Order;
using ECommerceCore.Models;
using ECommerceCore.Interfaces;
using ECommerceInfrastructure.Configurations.Data;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceInfrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(ApplicationDbContext context, ILogger<OrderRepository> logger)
        {
            _context = context;
            _logger = logger;
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
               
                CartItems = cartItems
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Order created successfully with ID: {OrderId}", order.OrderId);

            // Map CartItems to ItemsInCart DTO
            var itemsInCart = cartItems.Select(ci => new ItemsInCart
            {
                CartItemId = ci.CartItemId,
                CartMainImageUrl = ci.Product.MainImage,
                cartItemPrice = ci.Product.Price,
                quantity = ci.Quantity
            }).ToList();

            // Create the ReadCreateOrderDTO
            var readCreateOrderDTO = new ReadCreateOrderDTO
            {
              
                FName = order.FName,
                LName = order.LName,
                Phone = order.Phone,
                City = order.City,
                Street = order.Street,
                Area = order.Area,
                totalPriceBeforeShipping = order.totalPriceBeforeShipping,
                shippingPrice = order.shippingPrice,
                totalPrice = order.totalPrice,
                ItemsInCart = itemsInCart
            };

            _logger.LogInformation("Order creation process completed successfully.");

            return readCreateOrderDTO;
        }
    }
}
