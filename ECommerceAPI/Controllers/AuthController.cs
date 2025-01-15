using ECommerceCore.DTOs.Account;
using ECommerceCore.Interfaces;
using ECommerceCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;

        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = new User
                {
                    UserName = model.Name,  // Corrected here to match the DTO
                    Email = model.Email,
                    IsActive = false,
                    createdAt = DateTime.UtcNow
                };
                if (model.Password != model.ConfirmPassword)
                {
                    return BadRequest(new { Message = "كلمة المرور وتأكيد كلمة المرور لا تتطابقان." });
                }

                var result = await _authRepository.RegisterAsync(user, model.Password);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] loginDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var token = await _authRepository.LoginAsync(model.email, model.password);
            if (token == null)
                return Unauthorized(new { Message = "البريد الإلكتروني أو كلمة المرور غير صحيحة." });

            return Ok(new { Token = token });
        }

        [HttpGet("confirmemail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
        {
            try
            {
                var result = await _authRepository.ConfirmEmailAsync(email, token);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}
