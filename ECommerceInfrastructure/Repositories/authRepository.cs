using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ECommerceCore.DTOs.User.Account;
using ECommerceCore.Interfaces;
using ECommerceCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Transactions;
using static System.Runtime.InteropServices.JavaScript.JSType;


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
        public async Task<User> GetUserFromClaimsAsync(ClaimsPrincipal userClaims)
        {
            var email = userClaims.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Email claim not found");
            }

            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<Dictionary<string, string[]>> RegisterAsync(RegisterDTO registerDTO)
        {
            var errors = new Dictionary<string, string[]>();

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // التحقق من وجود البريد الإلكتروني مسبقًا
                var existingUser = await _userManager.FindByEmailAsync(registerDTO.Email);
                if (existingUser != null)
                {
                    errors.Add("Email", new[] { "البريد الإلكتروني مسجل بالفعل. يرجى استخدام بريد آخر." });
                    return errors;
                }
                // العثور على المنطقة الزمنية لفلسطين
                var palestinianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Gaza");

                // تحويل الوقت الحالي إلى توقيت فلسطين
                var currentPalestinianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, palestinianTimeZone);
                // إنشاء المستخدم
                var user = new User
                {
                    UserName = registerDTO.Name,
                    Email = registerDTO.Email,
                    CreatedAt = currentPalestinianTime,
                    IsActive = false, // Initially inactive until email is confirmed
                    Role = "User" // تعيين قيمة افتراضية
                };

                var result = await _userManager.CreateAsync(user, registerDTO.Password);
                if (!result.Succeeded)
                {
                    if (result.Errors.Any(e =>
                        e.Code.Contains("PasswordRequiresNonAlphanumeric") ||
                        e.Code.Contains("PasswordRequiresLower") ||
                        e.Code.Contains("PasswordRequiresUpper") ||
                        e.Code.Contains("PasswordRequiresDigit")))
                    {
                        errors.Add("Password", new[] { "يرجى إدخال كلمة مرور قوية تحتوي على حرف صغير، حرف كبير، رقم، ورمز خاص." });
                    }
                    else if (result.Errors.Any(e => e.Code.Contains("DuplicateUserName")))
                    {
                        errors.Add("Name", new[] { "اسم المستخدم مسجل مسبقًا. يرجى اختيار اسم آخر." });
                    }
                    return errors;
                }

                // إضافة المستخدم إلى جدول الأدوار
                await _userManager.AddToRoleAsync(user, "User");

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                user.EmailConfirmationExpiry = DateTime.UtcNow.AddDays(7);

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
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

                try
                {
                    EmailSettings.SendEmail(email);
                    transaction.Complete(); // تأكيد المعاملة فقط بعد نجاح الإرسال
                    return null;
                }
                catch (Exception ex)
                {
                    errors.Add("InternetError", new[] { "تعذر إنشاء الحساب. يرجى التحقق من اتصالك بالإنترنت والمحاولة مرة أخرى." });
                    return errors;
                }
            }
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

            if (user == null)
            {
                return "Email does not exist."; // رسالة خاصة بعدم وجود البريد
            }

            if (!user.EmailConfirmed || !user.IsActive)
            {
                return "Your account is not activated. Please check your email."; // رسالة خاصة بالحساب غير المفعل
            }

            // Generate an 8-digit numeric code
            var numericCode = GenerateNumericCode(8);

            // Store the numeric code and expiration date in a user token
            var palestinianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Gaza");
            var expirationDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddMinutes(10), palestinianTimeZone); // رمز صالح لمدة 10 دقائق

            await _userManager.SetAuthenticationTokenAsync(user, "Default", "PasswordReset", numericCode);
            await _userManager.SetAuthenticationTokenAsync(user, "Default", "PasswordResetExpiration", expirationDate.ToString("o"));

            // Send the code via email
            var emailMessage = new Email
            {
                Recivers = email,
                Subject = "Password Reset Code",
                Body = $"Your password reset code is: {numericCode}. It will expire at {expirationDate}."
            };

            EmailSettings.SendEmail(emailMessage);

            // Generate and return the token
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }
        public async Task<Dictionary<string, string[]>> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO, string token)
        {
            var errors = new Dictionary<string, string[]>();

            // تحقق من وجود المستخدم
            var user = await _userManager.FindByEmailAsync(resetPasswordDTO.Email);
            if (user == null)
            {
                errors.Add("Email", new[] { "No user associated with this email." });
                return errors;
            }

            // تحقق من صلاحية الكود
            var storedCode = await _userManager.GetAuthenticationTokenAsync(user, "Default", "PasswordReset");
            var storedExpirationDate = await _userManager.GetAuthenticationTokenAsync(user, "Default", "PasswordResetExpiration");
            if (storedCode != resetPasswordDTO.Code)
            {
                errors.Add("Code", new[] { "الكود المدخل غير صحيح. يرجى التأكد من إدخال الكود الصحيح." });
                return errors;
            }

            // العثور على المنطقة الزمنية لفلسطين
            var palestinianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Gaza");

            // تحويل الوقت الحالي إلى توقيت فلسطين
            var currentPalestinianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, palestinianTimeZone);

            // تحقق من تاريخ انتهاء الصلاحية
            if (DateTime.TryParse(storedExpirationDate, out DateTime expirationDate))
            {
                if (expirationDate < currentPalestinianTime)
                {
                    errors.Add("ExpiredCode", new[] { "رمز إعادة تعيين منتهي الصلاحية." });
                    return errors;
                }
            }


            // تحقق من قوة كلمة المرور الجديدة قبل محاولة تعيينها
            if (string.IsNullOrEmpty(resetPasswordDTO.NewPassword) ||
                !resetPasswordDTO.NewPassword.Any(char.IsLower) ||
                !resetPasswordDTO.NewPassword.Any(char.IsUpper) ||
                !resetPasswordDTO.NewPassword.Any(char.IsDigit) ||
                !resetPasswordDTO.NewPassword.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                errors.Add("NewPassword", new[] { "يرجى إدخال كلمة مرور قوية تحتوي على حرف صغير، حرف كبير، رقم، ورمز خاص." });
                return errors;
            }

            // حاول إعادة تعيين كلمة المرور
            var resetPassResult = await _userManager.ResetPasswordAsync(user, token, resetPasswordDTO.NewPassword);
            if (!resetPassResult.Succeeded)
            {
                errors.Add("General", resetPassResult.Errors.Select(e => e.Description).ToArray());
                return errors;
            }

            // حذف الرمز بعد النجاح
            await _userManager.RemoveAuthenticationTokenAsync(user, "Default", "PasswordReset");
            await _userManager.RemoveAuthenticationTokenAsync(user, "Default", "PasswordResetExpiration");

            return null; // نجاح العملية
        }



        // Change password

        public async Task<Dictionary<string, string[]>> ChangePasswordAsync(ClaimsPrincipal userClaims, ChangePasswordDTO changePasswordDTO)
        {
            var errors = new Dictionary<string, string[]>();

            // جلب المستخدم من Claims
            var user = await GetUserFromClaimsAsync(userClaims);
            if (user == null)
            {
                errors.Add("User", new[] { "User not found." });
                return errors;
            }

            // تحقق من قوة كلمة المرور الجديدة قبل محاولة تعيينها
            if (string.IsNullOrEmpty(changePasswordDTO.NewPassword) ||
                !changePasswordDTO.NewPassword.Any(char.IsLower) ||
                !changePasswordDTO.NewPassword.Any(char.IsUpper) ||
                !changePasswordDTO.NewPassword.Any(char.IsDigit) ||
                !changePasswordDTO.NewPassword.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                errors.Add("NewPassword", new[] { "يرجى إدخال كلمة مرور قوية تحتوي على حرف صغير، حرف كبير، رقم، ورمز خاص." });
                return errors;
            }

            // تحقق من صحة كلمة المرور القديمة
            var passwordCheck = await _userManager.CheckPasswordAsync(user, changePasswordDTO.OldPassword);
            if (!passwordCheck)
            {
                errors.Add("OldPassword", new[] { "كلمة المرور القديمة غير صحيحة." });
                return errors;
            }

            // تغيير كلمة المرور
            var result = await _userManager.ChangePasswordAsync(user, changePasswordDTO.OldPassword, changePasswordDTO.NewPassword);
            if (!result.Succeeded)
            {
                errors.Add("Password", result.Errors.Select(e => e.Description).ToArray());
                return errors;
            }

            return null; // No errors, password change successful
        }


    }
}