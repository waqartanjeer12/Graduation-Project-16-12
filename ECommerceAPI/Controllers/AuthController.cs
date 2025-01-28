using ECommerceCore.DTOs.User.Account;
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