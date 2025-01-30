using ECommerceCore.DTOs.User;
using ECommerceCore.DTOs.User.Account;
using ECommerceCore.Interfaces;
using ECommerceCore.Models;
using ECommerceInfrastructure.Configurations.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceInfrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;  // Inject IFileService
        private readonly ILogger<UserRepository> _logger;  // Inject ILogger
        private readonly UserManager<User> _userManager;


        public UserRepository(ApplicationDbContext context, IFileService fileService, ILogger<UserRepository> logger, UserManager<User> userManager)
        {
            _context = context;
            _fileService = fileService;
            _logger = logger;
            _userManager = userManager;
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

        public async Task<GetUserForUserDTO> GetUserByEmailAsync(String email)
        {
            _logger.LogInformation("جارِ جلب المستخدم بالمعرف: {Id}", email);

            try
            {
                var user = await _context.Users
                    .Where(u => u.Email == email && u.IsActive==true)
                    .Select(u => new GetUserForUserDTO
                    {
                        UserId = u.Id,
                        UserName = u.UserName,
                        Email = u.Email,
                        ImgUrl = u.Img,
                        PhoneNumber = u.PhoneNumber,
                        City = u.City,
                        Area = u.Area,
                        Street = u.Street,
                       
                    })
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    _logger.LogWarning("لم يتم العثور على المستخدم بالمعرف: {Id}", email);
                    return null;
                }

                

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء جلب المستخدم بالمعرف: {Id}", email);
                return null;
            }
        }
        public async Task<List<GetUsersForAdminDTO>> GetAllUsersAdminAsync()
        {
            _logger.LogInformation("جارِ جلب جميع المستخدمين");

            try
            {
                var users = await _context.Users
                    .Where(u=> u.IsActive == true)
                    .Select(u => new GetUsersForAdminDTO
                    {
                        UserId = u.Id,
                        ImgUrl = u.Img,
                        UserName = u.UserName,
                        Email = u.Email,
                        PhoneNumber = u.PhoneNumber,
                        City = u.City,
                        IsActive = u.IsActive,
                         createdAt = u.CreatedAt.ToString("dd MMMM yyyy، hh:mm tt", new System.Globalization.CultureInfo("ar-PS"))
                    })
                    .ToListAsync();

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء جلب جميع المستخدمين");
                return null;
            }
        }
        public async Task<Dictionary<string, string[]>> UpdateUserAsync(ClaimsPrincipal userClaims, UpdateUserInformationDTO dto)
        {
            var errors = new Dictionary<string, string[]>();

            try
            {
                var email = userClaims.FindFirstValue(ClaimTypes.Email);
                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogWarning("لم يتم العثور على البريد الإلكتروني في المطالبات.");
                    errors.Add("Email", new[] { "البريد الالكتروني غير موجود ." });
                    return errors;
                }

                var user = await _userManager.FindByEmailAsync(email);
                if (user == null || user.IsActive == false)
                {
                    _logger.LogWarning("لم يتم العثور على المستخدم بالمعرف: {Email}", email);
                    errors.Add("UserNotFound", new[] { "المستخدم غير موجود" });
                    return errors;
                }

                // Update basic properties
                user.UserName = dto.Name ?? user.UserName; // return left if not null and then right
                user.PhoneNumber = dto.PhoneNumber;
                user.City = dto.City;
                user.Area = dto.Area;
                user.Street = dto.Street;

                // Handle Img update
                if (dto.Img != null)
                {
                    try
                    {
                        // Delete the old image if exists
                        if (!string.IsNullOrEmpty(user.Img))
                        {
                            _fileService.DeleteFile(Path.GetFileName(user.Img), "images");
                        }

                        // Upload the new image
                        var imgFileName = await _fileService.UploadFileAsync(dto.Img, "images");
                        user.Img = $"/files/images/{imgFileName}";
                    }
                    catch (InvalidOperationException ex)
                    {
                        _logger.LogError(ex, "تنسيق الملف غير صالح لتحديث صورة المستخدم.");
                        errors.Add("Img", new[] { "يرجى تحميل صورة بتنسيق JPG أو PNG أو JPEG أو WEBP فقط" });
                        return errors;
                    }
                  
                }

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("تم تحديث المستخدم بنجاح بالبريد الإلكتروني: {Email}", email);
                
            }
           
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تحديث المستخدم بالبريد الإلكتروني: {Email}");
                errors.Add("GeneralError", new[] { "حدث خطأ أثناء تحديث المستخدم." });
            }

            return errors;
        }
    }
}
