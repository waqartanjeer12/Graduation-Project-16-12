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

            return Ok(new { Message = "تم التسجيل بنجاح! يرجى التحقق من بريدك الإلكتروني لتأكيد حسابك." });
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
            return Ok(new { Message = "تم تأكيد الإيميل بنجاح" });
        }

        [HttpPost("send-reset-password-link")]
        public async Task<IActionResult> SendResetPasswordLink([FromBody] ForgotPasswordDTO forgotPasswordDTO)
        {
            var response = await _authRepository.SendResetCodeAsync(forgotPasswordDTO.Email);

            if (response == "Email does not exist." || response == "Your account is not activated. Please check your email.")
            {
                return BadRequest(new { message = response });
            }


            // If everything is fine, return success message
            return Ok(new { message = "The code has been sent successfully.", token = response });
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            // تحقق من تطابق كلمة المرور الجديدة مع تأكيد كلمة المرور
            if (resetPasswordDTO.NewPassword != resetPasswordDTO.ConfirmPassword)
            {
                return BadRequest(new { errors = new { ConfirmPassword = new[] { "كلمة المرور وتأكيدها غير متطابقين." } } });
            }

            // استخراج الرمز من الهيدر
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // استدعاء دالة ResetPasswordAsync
            var result = await _authRepository.ResetPasswordAsync(resetPasswordDTO, token);

            // إذا كانت النتيجة تحتوي على أخطاء
            if (result != null)
            {
                return BadRequest(new { errors = result });
            }

            // إذا كانت العملية ناجحة
            return Ok(new { message = "تم تحديث كلمة المرور بنجاح." });
        }



        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
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

            var userClaims = HttpContext.User;
            var errorsFromService = await _authRepository.ChangePasswordAsync(userClaims, changePasswordDTO);

            if (errorsFromService != null)
            {
                return BadRequest(new { errors = errorsFromService });
            }

            return Ok(new { Message = "تم تغيير كلمة المرور بنجاح." });
        }
    }
}