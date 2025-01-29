using ECommerceCore.DTOs.User.Account;
using ECommerceCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
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

            var errorsFromService = await _authRepository.RegisterAsync(registerDTO);

            if (errorsFromService != null)
            {
                return BadRequest(new { errors = errorsFromService });
            }

            return Ok(new { Message = "تم التسجيل بنجاح! يرجى التحقق من بريدك الإلكتروني لتأكيد حسابك." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            try
            {
                var token = await _authRepository.LoginAsync(loginDTO);
                if (token.StartsWith("Email") || token.StartsWith("Invalid") || token.StartsWith("Your email is not confirmed "))
                {
                    return BadRequest(token);
                }
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
         [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            var result = await _authRepository.ConfirmEmailAsync(email, token);

            if (result != null)
            {
                return BadRequest(new { errors = result });
            }

            // Redirect to the specified URL after successful confirmation
            return Redirect("http://localhost:5173");
        }


        [HttpPost("send-reset-password-link")]
        public async Task<IActionResult> SendResetPasswordLink([FromBody] ForgotPasswordDTO forgotPasswordDTO)
        {
            var token = await _authRepository.SendResetCodeAsync(forgotPasswordDTO.Email);
            if (token.Contains("No active user"))
                return BadRequest(new { message = token });

            // Return the reset token for testing purposes
            return Ok(new { message = "Password reset code has been sent to your email.", token });
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            if (resetPasswordDTO.NewPassword != resetPasswordDTO.ConfirmPassword)
            {
                return BadRequest(new { message = "Password and Confirm Password do not match." });
            }

            // Extract the token from the Authorization header
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var result = await _authRepository.ResetPasswordAsync(resetPasswordDTO, token);
            if (result.Contains("successfully"))
                return Ok(new { message = result });
            return BadRequest(new { message = result });
        }
    }
}
