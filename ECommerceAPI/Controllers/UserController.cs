using ECommerceCore.DTOs.User;
using ECommerceCore.Interfaces;
using ECommerceInfrastructure.Repositories;
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

        [HttpGet("ByUserId")]

        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _repository.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(user);
        }
        [HttpGet("AllUsersAdmin")]
        public async Task<IActionResult> GetAllUsersAdmin()
        {
            var users = await _repository.GetAllUsersAdminAsync();

            if (users == null)
            {
                return NotFound(new { message = "No users found" });
            }

            return Ok(users);
        }
        [HttpPut("update/{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromForm] UpdateUserInformationDTO dto)
        {
            var updatedUser = await _repository.UpdateUserAsync(userId, dto);
            if (updatedUser == null)
            {
                return NotFound(new { message = "User not found." });
            }
            return Ok(updatedUser);
        }
    }
}
