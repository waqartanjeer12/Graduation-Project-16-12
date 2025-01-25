using ECommerceCore.DTOs.Cart;
using ECommerceInfrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartRepository _repository;

    public CartController(ICartRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddItemToCart([FromBody] CartAddItemsToCartDTO addItemDto)
    {
        var result = await _repository.AddItemToCartAsync(addItemDto);
        if (result == null)
        {
            return BadRequest("Item could not be added to the cart.");
        }
        return Ok(result);
    }
    [HttpGet("items")]
        public async Task<IActionResult> GetCartItem([FromQuery] string token)
        {
            var result = await _repository.GetAllCartItemsAsync(token);
            if (result == null)
            {
                return NotFound("No items found in the cart.");
            }
            return Ok(result);
        }



        [HttpDelete("Clear")]
        public async Task<IActionResult>  ClearCartItemsByItemIdsAsync([FromForm]int cartId,[FromForm] int[] itemIds)
        {
            // Call the repository method to remove the item from the cart
            var result = await _repository.ClearCartItemsByItemIdsAsync(cartId,itemIds);

            // If the item was not found or removed, return a 404 Not Found response
            if (!result)
            {
                return NotFound(new { message = "Item not found in the cart" });
            }

            // Return a 204 No Content response if the item was removed successfully
            return NoContent();

        }
        [HttpDelete("remove/{itemid}")]
        public async Task<IActionResult> RemoveItemFromCart(int itemid)
        {
            // Call the repository method to remove the item from the cart
            
            var result = await _repository.RemoveItemFromCartAsync(itemid);

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
