using ECommerceCore.DTOs.Cart;
using ECommerceInfrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

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
        var result = await _repository.AddItemToCartAsync(addItemDto, User);
        if (result == null)
        {
            return BadRequest("Item could not be added to the cart.");
        }
        return Ok(result);
    }

    [HttpGet("items")]
    public async Task<IActionResult> GetCartItem()
    {
        var result = await _repository.GetAllCartItemsAsync(User);
        if (result == null)
        {
            return NotFound("No items found in the cart.");
        }
        return Ok(result);
    }

    [HttpDelete("Clear")]
    public async Task<IActionResult> ClearCartItemsByItemIdsAsync([FromForm] int cartId, [FromForm] int[] itemIds)
    {
        var result = await _repository.ClearCartItemsByItemIdsAsync(cartId, itemIds);
        if (!result)
        {
            return NotFound(new { message = "Item not found in the cart" });
        }
        return NoContent();
    }

    [HttpDelete("remove/{itemid}")]
    public async Task<IActionResult> RemoveItemFromCart(int itemid)
    {
        var result = await _repository.RemoveItemFromCartAsync(itemid);
        if (!result)
        {
            return NotFound(new { message = "Item not found in the cart" });
        }
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