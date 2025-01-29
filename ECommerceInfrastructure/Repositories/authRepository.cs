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
            // Adding essential claims like name, email, and role
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email), // Email
                new Claim(JwtRegisteredClaimNames.Email, user.Email), // Email (duplicate for JWT standard compliance)
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // User ID
                new Claim("userName", user.UserName), // Username
            };
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                claims.Add(new Claim("role", role));
            }
            // Creating the signing key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JWT")["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            // Creating the token
            var token = new JwtSecurityToken(
                issuer: _configuration.GetSection("JWT")["Issuer"],
                audience: _configuration.GetSection("JWT")["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(14), // Token validity: 14 days
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
      

        private string GenerateNumericCode(int length)
        {
            var random = new Random();
            var code = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                code.Append(random.Next(0, 10));
            }
            return code.ToString();
        }

        public async Task<string> SendResetCodeAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !user.IsActive)
            {
                return "No active user associated with this email.";
            }

            // Generate an 8-digit numeric code
            var numericCode = GenerateNumericCode(8);

            // Generate a token for password reset
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Store the numeric code in a user token
            await _userManager.SetAuthenticationTokenAsync(user, "Default", "PasswordReset", numericCode);

            // Send the code via email
            var emailMessage = new Email
            {
                Recivers = email,
                Subject = "Password Reset Code",
                Body = $"Your password reset code is: {numericCode}"
            };

            EmailSettings.SendEmail(emailMessage);

            // Return the token for testing purposes (in a real application, handle securely)
            return token;
        }

        public async Task<string> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO, string token)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDTO.Email);
            if (user == null)
            {
                return "No user associated with this email.";
            }

            // Verify the numeric code
            var storedCode = await _userManager.GetAuthenticationTokenAsync(user, "Default", "PasswordReset");
            if (storedCode != resetPasswordDTO.Code)
            {
                return "Invalid or expired reset code.";
            }

            var resetPassResult = await _userManager.ResetPasswordAsync(user, token, resetPasswordDTO.NewPassword);
            if (!resetPassResult.Succeeded)
            {
                return string.Join(", ", resetPassResult.Errors.Select(e => e.Description));
            }

            // Clear the token after successful password reset
            await _userManager.RemoveAuthenticationTokenAsync(user, "Default", "PasswordReset");

            return "Password has been reset successfully.";
        }
    }
}
