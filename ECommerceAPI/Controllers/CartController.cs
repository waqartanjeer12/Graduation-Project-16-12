using ECommerceCore.DTOs.Cart;
using ECommerceInfrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

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
    [Authorize(Roles = "User")]
    public async Task<IActionResult> AddItemToCart([FromBody] CartAddItemsToCartDTO addItemDto)
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

        var errorsFromService = await _repository.AddItemToCartAsync(addItemDto, User);

        if (errorsFromService != null)
        {
            return BadRequest(new { errors = errorsFromService });
        }

        return Ok(new { Message = "Item added to cart successfully." });
    }

    [HttpGet("items")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetCartItem()
    {
        var result = await _repository.GetAllCartItemsAsync(User);
        if (result == null)
        {
            return NotFound("No items found in the cart.");
        }
        return Ok(result);
    }
    [HttpDelete("clear")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> ClearCartItemsByItemIdsAsync([FromForm] int[] itemIds)
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

        var errorsFromService = await _repository.ClearCartItemsByItemIdsAsync(User, itemIds);

        if (errorsFromService != null)
        {
            return BadRequest(new { errors = errorsFromService });
        }

        return Ok(new { Message = "Items deleted from cart successfully." });
    }

    [HttpDelete("remove/{cartitemid}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> RemoveItemFromCart([FromForm] int cartitemid)
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

        var errorsFromService = await _repository.RemoveItemFromCartAsync(User, cartitemid);

        if (errorsFromService != null)
        {
            return NotFound(new { errors = errorsFromService });
        }

        return Ok(new { Message = "Item deleted from cart successfully." });
    }

    [HttpPost("increase-quantity")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> IncreaseQuantity([FromForm] int itemId)
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

        var errorsFromService = await _repository.IncreaseQuantityAsync(User, itemId);

        if (errorsFromService != null)
        {
            return BadRequest(new { errors = errorsFromService });
        }

        return Ok(new { message = "Quantity increased successfully." });
    }

    [HttpPost("decrease-quantity")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> DecreaseQuantity([FromForm] int itemId)
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

        var errorsFromService = await _repository.DecreaseQuantityAsync(User, itemId);

        if (errorsFromService != null)
        {
            return BadRequest(new { errors = errorsFromService });
        }

        return Ok(new { message = "Quantity decreased successfully." });
    }
}