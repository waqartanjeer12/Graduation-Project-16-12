using ECommerceCore.Interfaces;
using ECommerceCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommerceInfrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;

        public AuthRepository(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailSender = emailSender;
        }

        public async Task<string> RegisterAsync(User user, string password)
        {
            try
            {
                var result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

                // إنشاء رمز تأكيد البريد الإلكتروني
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                user.EmailConfirmationExpiry = DateTime.UtcNow.AddDays(7);

                // تحديث المستخدم لإضافة تاريخ انتهاء صلاحية التأكيد
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                    throw new Exception(string.Join(", ", updateResult.Errors.Select(e => e.Description)));

                try
                {
                    // إرسال رابط التأكيد
                    var confirmationLink = $"{_configuration["AppUrl"]}/api/auth/confirmemail?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(token)}";
                    await _emailSender.SendEmailAsync(user.Email, "تأكيد بريدك الإلكتروني", $"<p>لتأكيد بريدك الإلكتروني اضغط <a href='{confirmationLink}'>هنا</a>.</p>");
                }
                catch (Exception emailException)
                {
                    throw new Exception("حدث خطأ أثناء إرسال البريد الإلكتروني: " + emailException.Message);
                }

                return "تم التسجيل بنجاح! يرجى التحقق من بريدك الإلكتروني لتأكيد الحساب.";
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException != null)
                {
                    throw new Exception($"خطأ في تحديث قاعدة البيانات: {dbEx.InnerException.Message}");
                }
                throw new Exception("حدث خطأ غير متوقع في قاعدة البيانات أثناء عملية التسجيل.");
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء عملية التسجيل: " + ex.Message);
            }
        }

        public async Task<string> ConfirmEmailAsync(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) throw new Exception("User not found.");

            if (user.IsActive) return "Email already confirmed.";

            if (user.EmailConfirmationExpiry < DateTime.UtcNow)
                throw new Exception("Email confirmation link has expired.");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded) throw new Exception("Failed to confirm email.");

            user.IsActive = true;
            await _userManager.UpdateAsync(user);

            return "Email confirmed successfully.";
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !user.IsActive)
                return "البريد الإلكتروني أو كلمة المرور غير صحيحة.";

            var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
            if (!result.Succeeded)
                return "البريد الإلكتروني أو كلمة المرور غير صحيحة.";

            return GenerateToken(user);
        }

        private string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
