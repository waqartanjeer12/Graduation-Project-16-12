using ECommerceInfrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ECommerceCore.DTOs.Order;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _repository;

        public OrderController(IOrderRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("create")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateOrder([FromForm] CreateOrderDTO createOrderDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(ms => ms.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return BadRequest(new { errors });
            }

            var result = await _repository.CreateOrderAsync(createOrderDTO, User);

            if (result != null && result.Any())
            {
                return BadRequest(new { errors = result });
            }

            return Ok(new { message = "تم انشاء الطلب بنجاح" });
        }

       

        [HttpGet("{orderId}/user-orders")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetUserOrdersById(int orderId)
        {
            var order = await _repository.GetUserOrdersByIdAsync(orderId);

            if (order == null)
            {
                ModelState.AddModelError("Order", "Order not found.");
                var errors = ModelState
                    .Where(ms => ms.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return BadRequest(new { errors });
            }

            return Ok(order);
        }

        [HttpGet("{orderId}/order-details")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetOrderDetailsById(int orderId)
        {
            var orderDetail = await _repository.GetOrderDetailsByIdAsync(orderId);

            if (orderDetail == null)
            {
                ModelState.AddModelError("Order", "Order not found.");
                var errors = ModelState
                    .Where(ms => ms.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return BadRequest(new { errors });
            }

            return Ok(orderDetail);
        }

        [HttpGet("user/{userId}/orders")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetUserOrdersByUserId(int userId)
        {
            var orders = await _repository.GetUserOrdersByUserIdAsync(userId);

            if (orders == null || !orders.Any())
            {
                ModelState.AddModelError("Orders", "No orders found for this user.");
                var errors = ModelState
                    .Where(ms => ms.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return BadRequest(new { errors });
            }

            return Ok(orders);
        }

        [HttpGet("admin/all-user-orders")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUserOrdersForAdmin()
        {
            var orders = await _repository.GetAllUserOrdersForAdminAsync();

            if (orders == null || !orders.Any())
            {
                ModelState.AddModelError("Orders", "No orders found.");
                var errors = ModelState
                    .Where(ms => ms.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return BadRequest(new { errors });
            }

            return Ok(orders);
        }

        [HttpGet("admin/{orderId}/order-details")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrderDetailsForAdminById(int orderId)
        {
            var orderDetail = await _repository.GetOrderDetailsForAdminByIdAsync(orderId);

            if (orderDetail == null)
            {
                ModelState.AddModelError("Order", "Order not found.");
                var errors = ModelState
                    .Where(ms => ms.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return BadRequest(new { errors });
            }

            return Ok(orderDetail);
        }

        [HttpPut("{orderId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatusById(int orderId, [FromBody] UpdateOrderStatusDTO updateOrderStatusDTO)
        {
            var result = await _repository.UpdateOrderStatusByIdAsync(orderId, updateOrderStatusDTO);

            if (result != null)
            {
                foreach (var error in result)
                {
                    ModelState.AddModelError(error.Key, string.Join(", ", error.Value));
                }
                var errorsDict = ModelState
                    .Where(ms => ms.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                return BadRequest(errorsDict);
            }

            return Ok(new { Message = "تم تحديث حالة الطلب بنجاح" });
        }
    }
}