using System;
using System.Linq;
using System.Threading.Tasks;
using ECommerceCore.DTOs.Account;
using ECommerceCore.Interfaces;
using ECommerceCore.Models;
using Microsoft.AspNetCore.Identity;
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

        public AuthRepository(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<string> RegisterAsync(RegisterDTO registerDTO)
        {
            try
            {
                var user = new User
                {
                    UserName = registerDTO.Name,
                    Email = registerDTO.Email,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = false  // Initially inactive until email is confirmed
                };

                var result = await _userManager.CreateAsync(user, registerDTO.Password);
                if (!result.Succeeded)
                    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

                // Generate email confirmation token
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                user.EmailConfirmationExpiry = DateTime.UtcNow.AddDays(7);

                // Update user to add email confirmation expiry date
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                    throw new Exception(string.Join(", ", updateResult.Errors.Select(e => e.Description)));

                try
                {
                    var confirmationLink = $"{_configuration["AppUrl"]}/api/auth/confirm-email?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(token)}";
                    var email = new Email()
                    {
                        Subject = "Email Confirmation",
                        Recivers = user.Email,
                        Body = $"Please confirm your email by clicking <a href='{confirmationLink}'>here</a>."
                    };
                    EmailSettings.SendEmail(email);
                }
                catch (Exception emailException)
                {
                    throw new Exception("There was an error sending the email: " + emailException.Message);
                }

                return "Registration successful! Please check your email to confirm your account.";
            }
            catch (Exception ex)
            {
                throw new Exception("There was an error during the registration process: " + ex.Message);
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

        public async Task<string> LoginAsync(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.email);
            if (user == null || !user.IsActive)
                return "Invalid email or password.";

            var result = await _signInManager.PasswordSignInAsync(user, loginDTO.password, loginDTO.rememberMe, false);
            if (!result.Succeeded)
                return "Invalid email or password.";

            return GenerateToken(user);
        }

        public async Task<string> SendEmailConfirmationLink(ForgotPassword forgotPassword)
        {
            var user = await _userManager.FindByEmailAsync(forgotPassword.email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var email = new Email()
                {
                    Subject = "Reset Password",
                    Recivers = forgotPassword.email,
                    Body = $"To reset your password, click <a href='{_configuration["AppUrl"]}/reset-password?email={forgotPassword.email}&token={Uri.EscapeDataString(token)}'>here</a>."
                };
                EmailSettings.SendEmail(email);
            }

            return "Password reset link sent successfully.";
        }

        public async Task<string> ResetPasswordAsync(ResetPasswordDTO resetPassword)
        {
            var user = await _userManager.FindByEmailAsync(resetPassword.Email);
            if (user == null)
                return "User not found.";
            var result = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.NewPassword);
            if (!result.Succeeded)
                return "Failed to reset password.";
            return "Password reset successfully.";
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