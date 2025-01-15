using ECommerceCore.DTOs.Cart;
using ECommerceInfrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _repository;

        public CartController(ICartRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddItemToCart([FromForm] CartAddItemsToCartDTO addItemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cartReadAddItemsToCartDto = await _repository.AddItemToCartAsync(addItemDto);

            if (cartReadAddItemsToCartDto == null)
            {
                return NotFound(new { message = "Product or color not found" });
            }

           

            return Ok(cartReadAddItemsToCartDto);
        }


        [HttpGet("all")]
        public async Task<ActionResult<CartGetAllItemsDTO>> GetAllCartItems()
        {
            var cartItems = await _repository.GetAllCartItemsAsync();

            if (cartItems == null)
            {
                return NotFound(new { message = "Cart not found" });
            }

            return Ok(cartItems);
        }

        [HttpDelete("{cartId}/clear")]
        public async Task<IActionResult> ClearCart(int cartId)
        {
            // Call the repository method to clear the cart
            var result = await _repository.ClearCartAsync(cartId);

            // If the cart was not found, return a 404 Not Found response
            if (!result)
            {
                return NotFound(new { message = "Cart not found" });
            }

            // Return a 204 No Content response if the cart was cleared successfully
            return NoContent();
        }

        [HttpDelete("remove/{productId}")]
        public async Task<IActionResult> RemoveItemFromCart(int productId)
        {
            // Call the repository method to remove the item from the cart
            var result = await _repository.RemoveItemFromCartAsync(productId);

            // If the item was not found or removed, return a 404 Not Found response
            if (!result)
            {
                return NotFound(new { message = "Item not found in the cart" });
            }

            // Return a 204 No Content response if the item was removed successfully
            return NoContent();
        }
        [HttpPost("increase-quantity")]
        public async Task<IActionResult> IncreaseQuantity([FromForm] int itemId)
        {
           
            var success = await _repository.IncreaseQuantityAsync(itemId);
            if (!success)
            {
                return BadRequest(new { message = "Failed to increase quantity. Item not found or not enough stock." });
            }

            return Ok(new { message = "Quantity increased successfully." });
        }

        [HttpPost("decrease-quantity")]
        public async Task<IActionResult> DecreaseQuantity([FromForm] int itemId)
        {
           

            var success = await _repository.DecreaseQuantityAsync(itemId);
            if (!success)
            {
                return BadRequest(new { message = "Failed to decrease quantity. Item not found or quantity cannot be less than 1." });
            }

            return Ok(new { message = "Quantity decreased successfully." });
        }
    }
}