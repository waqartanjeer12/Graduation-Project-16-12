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
        [HttpGet("all-orders")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrdersForAdmin()
        {
            var orders = await _repository.GetAllOrdersForAdminAsync();

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
        [HttpGet("user")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetOrderDetailsByUser()
        {
            var userClaims = HttpContext.User;
            var orderDetails = await _repository.GetOrderDetailsByUserAsync(userClaims);

            if (orderDetails == null)
            {
                ModelState.AddModelError("Orders", "No orders found for the user.");
                var errors = ModelState
                    .Where(ms => ms.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                return BadRequest(errors);
            }

            return Ok(orderDetails);
        }

        // Endpoint to fetch order details by order ID
        [HttpGet("{orderId}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetOrderDetailsById(int orderId)
        {
            var orderDetails = await _repository.GetOrderDetailsByIdAsync(orderId);

            if (orderDetails == null)
            {
                ModelState.AddModelError("Order", "Order not found.");
                var errors = ModelState
                    .Where(ms => ms.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                return BadRequest(errors);
            }

            return Ok(orderDetails);
        }


        [HttpPut("user/status")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> UpdateOrderStatusByUser([FromBody] UpdateOrderStatusDTO updateOrderStatusDTO)
        {
            var userClaims = HttpContext.User;
            var result = await _repository.UpdateOrderStatusByUserAsync(userClaims, updateOrderStatusDTO);

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

           return Ok(new { Message = "تم تحديث حالة الطلب بنجاح." }); ; // Success with no content
        }

        [HttpPut("{orderId}/status")]
        [Authorize(Roles ="Admin")]
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

            return Ok(new { Message = "تم تحديث حالة الطلب بنجاح" }); // Success with no content
        }
    }
}