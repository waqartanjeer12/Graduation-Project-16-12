using ECommerceCore.DTOs.Account;
using ECommerceCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            var result = await _authRepository.RegisterAsync(registerDTO);
            if (result.Contains("successful"))
                return Ok(new { message = result });
            return BadRequest(new { message = result });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            if (loginDTO == null)
            {
                return BadRequest(new { message = "The loginDTO field is required." });
            }

            var result = await _authRepository.LoginAsync(loginDTO);
            if (result.StartsWith("eyJ"))  // JWT token usually starts with "eyJ"
                return Ok(new { token = result });
            return BadRequest(new { message = result });
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            var result = await _authRepository.ConfirmEmailAsync(email, token);
            if (result.Contains("successfully"))
                // Redirect to the specified URL after successful confirmation
                return Redirect("http://localhost:5173");
            return BadRequest(new { message = result });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO forgotPasswordDTO)
        {
            var result = await _authRepository.ForgotPasswordAsync(forgotPasswordDTO);
            if (result.Contains("sent"))
                return Ok(new { message = result });
            return BadRequest(new { message = result });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authRepository.ResetPasswordAsync(resetPasswordDTO);
            if (result.Contains("successfully"))
                return Ok(new { message = result });
            return BadRequest(new { message = result });
        }
    }
}