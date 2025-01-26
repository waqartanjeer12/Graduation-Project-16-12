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

        public async Task<string> RegisterAsync(RegisterDTO registerDTO)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerDTO.Email);
            if (existingUser != null)
            {
                if (!existingUser.EmailConfirmed || !existingUser.IsActive)
                {
                    return "Email already exists but is not confirmed or the account is inactive. Please check your email to confirm your account.";
                }
                return "Email already exists.";
            }

            var user = new User
            {
                UserName = registerDTO.Name,
                Email = registerDTO.Email,
                CreatedAt = DateTime.UtcNow,
                IsActive = false // Initially inactive until email is confirmed
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            user.EmailConfirmationExpiry = DateTime.UtcNow.AddDays(7);

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                throw new Exception(string.Join(", ", updateResult.Errors.Select(e => e.Description)));

            var confirmationLink = $"{_configuration["AppUrl"]}/api/auth/confirm-email?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(token)}";
            var email = new Email()
            {
                Subject = "Email Confirmation",
                Recivers = user.Email,
                Body = $"Please confirm your email by clicking <a href='{confirmationLink}'>here</a>."
            };
            EmailSettings.SendEmail(email);

            return "Registration successful! Please check your email to confirm your account.";
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
            string role = user.Email == "waqar.tanger12@gmail.com" ? "Admin" : "User";

            // Adding essential claims like name, email, and role
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email), // Email
                new Claim(JwtRegisteredClaimNames.Email, user.Email), // Email (duplicate for JWT standard compliance)
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // User ID
                new Claim(ClaimTypes.Name, user.UserName), // Username
                new Claim(ClaimTypes.Role, role) // User's role
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