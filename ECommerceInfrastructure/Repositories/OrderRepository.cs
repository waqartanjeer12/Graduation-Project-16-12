using ECommerceCore.DTOs.Order;
using ECommerceCore.Models;
using ECommerceCore.Interfaces;
using ECommerceInfrastructure.Configurations.Data;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ECommerceCore.DTOs.Color;
using MailKit.Search;

namespace ECommerceInfrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrderRepository> _logger;
        private readonly UserManager<User> _userManager;

        public OrderRepository(ApplicationDbContext context, ILogger<OrderRepository> logger, UserManager<User> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<User> GetUserFromClaimsAsync(ClaimsPrincipal userClaims)
        {
            var email = userClaims.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Email claim not found");
            }

            return await _userManager.FindByEmailAsync(email);
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

            // العثور على المنطقة الزمنية لفلسطين
            var palestinianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Gaza");

            // تحويل الوقت الحالي إلى توقيت فلسطين
            var currentPalestinianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, palestinianTimeZone);

            // Create the Order entity
            var order = new Order
            {
                UserId = user.Id,
                orderDate = currentPalestinianTime,
                orderStatus = "تم الاستلام",
                orderStatusDetails = "تم استلام طلبك بنجاح. سيتم تجهيزه قريبًا.",
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





        public async Task<ReadOrderDetail> GetOrderDetailsByIdAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ThenInclude(p => p.Colors)
                .ThenInclude(pc => pc.Color)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return null;
            }

            var user = await _userManager.FindByIdAsync(order.UserId.ToString());

            var orderDetail = new ReadOrderDetail
            {
                OrderNumber = 123456 + order.OrderId, // Sequential number starting from 123456
                orderDate = order.orderDate.ToString("dd MMMM yyyy، hh:mm tt", new System.Globalization.CultureInfo("ar-PS")),
                orderStatus = order.orderStatus,
                totalPriceBeforeShipping = order.totalPriceBeforeShipping,
                shippingPrice = order.shippingPrice,
                totalPrice = order.totalPrice,
                CustomerName = $"{order.FName} {order.LName}", // Combine first name and last name
                Phone = order.Phone,
                City = order.City,
                Street = order.Street,
                Area = order.Area,
                OrderProducts = order.OrderItems.Select(oi => new OrderProducts
                {
                    Id = oi.ProductId,
                    MainImageUrl = oi.OrderItemMainImageUrl,
                    Name = oi.Product.Name,
                    Quantity = oi.Quantity,
                    OnePiecePrice = oi.OrderItemPrice,
                    totalPricewithQuantit = oi.OrderItemPrice * oi.Quantity,
                    Color = oi.Product.Colors.Select(pc => new ColorReadDTO
                    {
                        Id = pc.Color.Id,
                        ColorImage = pc.Color.Image,
                        Name = pc.Color.Name
                    }).FirstOrDefault()
                }).ToList()
            };

            return orderDetail;
        }

        public async Task<Dictionary<string, string[]>> UpdateOrderStatusByIdAsync(int orderId, UpdateOrderStatusDTO updateOrderStatusDTO)
        {
            var errors = new Dictionary<string, string[]>();
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                errors.Add("Order", new[] { "Order not found." });
                return errors;
            }

            order.orderStatus = updateOrderStatusDTO.OrderStatus;
            order.orderStatusDetails = updateOrderStatusDTO.OrderStatusDetails;

            await _context.SaveChangesAsync();
            return null; // No errors
        }

        public async Task<List<ReadUserOrders>> GetUserOrdersByEmailAsync(string email)
        {
            // البحث عن UserId باستخدام الـ email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                // في حالة لم يتم العثور على المستخدم باستخدام الـ email
                throw new Exception("User not found.");
            }

            // استخدام الـ userId للبحث عن الطلبات
            var orders = await _context.Orders
                .Where(o => o.UserId == user.UserId)
                .Select(o => new ReadUserOrders
                {
                    OrderId = o.OrderId,
                    OrderNumber = 123456 + o.OrderId, // Sequential number starting from 123456
                    orderDate = o.orderDate.ToString("dd MMMM yyyy، hh:mm tt", new System.Globalization.CultureInfo("ar-PS")),
                    orderStatus = o.orderStatus,
                    orderStatusDetails = o.orderStatusDetails,
                    orderTotalPrice = o.totalPrice
                })
                .ToListAsync();

            return orders;
        }

        public async Task<List<ReadUserOrdersForAdmin>> GetAllUserOrdersForAdminAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Select(o => new ReadUserOrdersForAdmin
                {
                    OrderId = o.OrderId,
                    OrderNumber = 123456 + o.OrderId, // Sequential number starting from 123456
                    UserName = o.User.UserName,
                    orderDate = o.orderDate.ToString("dd MMMM yyyy، hh:mm tt", new System.Globalization.CultureInfo("ar-PS")),
                    orderStatus = o.orderStatus,
                    orderTotalPrice = o.totalPrice
                })
                .ToListAsync();

            return orders;
        }


        public async Task<ReadOrderDetailsForAdmin> GetOrderDetailsForAdminByIdAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ThenInclude(p => p.Colors)
                .ThenInclude(pc => pc.Color)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return null;
            }

            var orderDetail = new ReadOrderDetailsForAdmin
            {
                OrderNumber = 123456 + order.OrderId, // Sequential number starting from 123456
                UserName = order.User.UserName, // UserName from User entity
                orderDate = order.orderDate.ToString("dd MMMM yyyy، hh:mm tt", new System.Globalization.CultureInfo("ar-PS")),  // تنسيق تاريخ الطلب,
                orderStatus = order.orderStatus,
                totalPriceBeforeShipping = order.totalPriceBeforeShipping,
                shippingPrice = order.shippingPrice,
                totalPrice = order.totalPrice,
                CustomerName = $"{order.FName} {order.LName}", // Combine first name and last name
                Phone = order.Phone,
                City = order.City,
                Street = order.Street,
                Area = order.Area,
                OrderProducts = order.OrderItems.Select(oi => new OrderProducts
                {
                    Id = oi.ProductId,
                    MainImageUrl = oi.OrderItemMainImageUrl,
                    Name = oi.Product.Name,
                    Quantity = oi.Quantity,
                    OnePiecePrice = oi.OrderItemPrice,
                    totalPricewithQuantit = oi.OrderItemPrice * oi.Quantity,
                    Color = oi.Product.Colors.Select(pc => new ColorReadDTO
                    {
                        Id = pc.Color.Id,
                        ColorImage = pc.Color.Image,
                        Name = pc.Color.Name
                    }).FirstOrDefault()
                }).ToList()
            };

            return orderDetail;
        }

        public async Task<Dictionary<string, string[]>> DeleteOrderByIdAsync(int orderId)
        {
            var errors = new Dictionary<string, string[]>();
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                errors.Add("Order", new[] { "Order not found." });
                return errors;
            }

            // حذف العناصر المرتبطة بالطلب (إن وجد)
            var orderItems = await _context.OrderItems.Where(oi => oi.OrderId == orderId).ToListAsync();
            _context.OrderItems.RemoveRange(orderItems);

            // حذف الطلب
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return null; // No errors
        }
    }


}