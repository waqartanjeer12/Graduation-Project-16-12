using ECommerceCore.DTOs.User;
using ECommerceCore.Interfaces;
using ECommerceInfrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repository;

        public UserController(IUserRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("ByUserEmail")]

        [Authorize]
        public async Task<IActionResult> GetUserByEmail(String email)
        {
            var user = await _repository.GetUserByEmailAsync(email);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(user);
        }
        [HttpGet("AllUsersAdmin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsersAdmin()
        {
            var users = await _repository.GetAllUsersAdminAsync();

            if (users == null)
            {
                return NotFound(new { message = "No users found" });
            }

            return Ok(users);
        }
        [HttpPut("update")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateUser([FromForm] UpdateUserInformationDTO dto)
        {
            var userClaims = HttpContext.User;
            var result = await _repository.UpdateUserAsync(userClaims, dto);
            
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

           

            if (result == null)
            {
                return BadRequest(errorsDict); 
            }


            return Ok(new { message = "تم تحديث المستخدم بنجاح." });

        }
    }
}