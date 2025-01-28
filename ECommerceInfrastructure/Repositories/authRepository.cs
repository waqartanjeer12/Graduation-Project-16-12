using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ECommerceCore.DTOs.User.Account;
using ECommerceCore.Interfaces;
using ECommerceCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ECommerceInfrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthRepository(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<Dictionary<string, string[]>> RegisterAsync(RegisterDTO registerDTO)
        {
            var errors = new Dictionary<string, string[]>();

            // التحقق من وجود البريد الإلكتروني مسبقًا
            var existingUser = await _userManager.FindByEmailAsync(registerDTO.Email);
            if (existingUser != null)
            {
                errors.Add("Email", new[] { "البريد الإلكتروني مسجل بالفعل. يرجى استخدام بريد آخر." });
                return errors;
            }

            // إنشاء المستخدم
            var user = new User
            {
                UserName = registerDTO.Name,
                Email = registerDTO.Email,
                CreatedAt = DateTime.UtcNow,
                IsActive = false, // Initially inactive until email is confirmed
                Role = "User" // تعيين قيمة افتراضية
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);
            if (!result.Succeeded)
            {
                // إضافة أخطاء كلمة المرور
                if (result.Errors.Any(e =>
                    e.Code.Contains("PasswordRequiresNonAlphanumeric") ||
                    e.Code.Contains("PasswordRequiresLower") ||
                    e.Code.Contains("PasswordRequiresUpper") ||
                    e.Code.Contains("PasswordRequiresDigit")))
                {
                    errors.Add("Password", new[] { "يرجى إدخال كلمة مرور قوية تحتوي على حرف صغير، حرف كبير، رقم، ورمز خاص." });
                }
                // إضافة أخطاء الاسم
                else if (result.Errors.Any(e => e.Code.Contains("DuplicateUserName")))
                {
                    errors.Add("Name", new[] { "اسم المستخدم مسجل مسبقًا. يرجى اختيار اسم آخر." });
                }
                return errors;
            }



            // add user to role table
            await _userManager.AddToRoleAsync(user, "User");


            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            user.EmailConfirmationExpiry = DateTime.UtcNow.AddDays(7);

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                // إضافة أخطاء التحديث
                errors.Add("General", updateResult.Errors.Select(e => e.Description).ToArray());
                return errors;
            }

            // إرسال بريد التأكيد
            var confirmationLink = $"{_configuration["AppUrl"]}/api/auth/confirm-email?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(token)}";
            var email = new Email()
            {
                Subject = "Email Confirmation",
                Recivers = user.Email,
                Body = $"Please confirm your email by clicking <a href='{confirmationLink}'>here</a>."
            };
            EmailSettings.SendEmail(email);
            // إذا لم توجد أخطاء
            return null;

        }
        public async Task<Dictionary<string, string[]>> ConfirmEmailAsync(string email, string token)
        {
            var errors = new Dictionary<string, string[]>();

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                errors.Add("Email", new[] { "المستخدم غير موجود" });
                return errors;
            }
            if (user.IsActive)
            {
                errors.Add("Email", new[] { "Email already confirmed." });
                return errors;
            }
            if (user.EmailConfirmationExpiry < DateTime.UtcNow)
            {
                errors.Add("Email", new[] { "Email confirmation link has expired." });
                return errors;
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                errors.Add("Email", result.Errors.Select(e => e.Description).ToArray());
                return errors;
            }

            user.IsActive = true;
            await _userManager.UpdateAsync(user);
            return null; // No errors, confirmation successful
        }
        public async Task<string> LoginAsync(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null)
                return "Email does not exist. You can register.";

            if (!user.EmailConfirmed || !user.IsActive)
                return "Your email is not confirmed or your account is inactive. Please confirm your email to activate your account.";

            var result = await _signInManager.PasswordSignInAsync(user, loginDTO.Password, loginDTO.RememberMe, false);
            if (!result.Succeeded)
                return "Invalid password.";

            return await GenerateTokenAsync(user);
        }

        private async Task<string> GenerateTokenAsync(User user)
        {
            // Determine the role
            var roles =await _userManager.GetRolesAsync(user);
            string role = roles.FirstOrDefault() ?? "User";

            // Adding essential claims like name, email, and role
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email), // Email
                new Claim(JwtRegisteredClaimNames.Email, user.Email), // Email (duplicate for JWT standard compliance)
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // User ID
                new Claim("userName", user.UserName), // Username
                new Claim("role", role) // User's role
            };

            // Creating the signing key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Creating the token
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(14), // Token validity: 14 days
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

      

        public async Task<string> ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDTO)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDTO.Email);
            if (user == null)
            {
                return "No user associated with this email.";
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"localhost/reset-password?email={Uri.EscapeDataString(forgotPasswordDTO.Email)}&token={Uri.EscapeDataString(token)}";

            var email = new Email
            {
                Recivers = forgotPasswordDTO.Email,
                Subject = "Password Reset",
                Body = $"Please reset your password by clicking here: <a href='{resetLink}'>link</a>"
            };

            EmailSettings.SendEmail(email);

            return "Password reset link sent";
        }

        public async Task<string> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDTO.Email);
            if (user == null)
            {
                return "No user associated with this email.";
            }

            var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPasswordDTO.Token, resetPasswordDTO.NewPassword);
            if (!resetPassResult.Succeeded)
            {
                return string.Join(", ", resetPassResult.Errors.Select(e => e.Description));
            }

            return "Password has been reset successfully.";
        }
    }
}