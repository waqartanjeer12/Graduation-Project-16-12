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

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            var result = await _authRepository.ConfirmEmailAsync(email, token);
            if (result.Contains("successfully"))
                return Ok(new { message = result });
            return BadRequest(new { message = result });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var result = await _authRepository.LoginAsync(loginDTO);
            if (result.StartsWith("eyJ"))  // JWT token usually starts with "eyJ"
                return Ok(new { token = result });
            return BadRequest(new { message = result });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPassword forgotPassword)
        {
            var result = await _authRepository.SendEmailConfirmationLink(forgotPassword);
            return Ok(new { message = result });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPassword)
        {
            var result = await _authRepository.ResetPasswordAsync(resetPassword);
            if (result.Contains("successfully"))
                return Ok(new { message = result });
            return BadRequest(new { message = result });
        }
    }
}