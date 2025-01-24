using ECommerceCore.DTOs.User;
using ECommerceCore.Interfaces;
using ECommerceInfrastructure.Configurations.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceInfrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;  // Inject IFileService
        private readonly ILogger<UserRepository> _logger;  // Inject ILogger

        public UserRepository(ApplicationDbContext context, IFileService fileService, ILogger<UserRepository> logger)
        {
            _context = context;
            _fileService = fileService;
            _logger = logger;
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
                        createdAt = u.CreatedAt
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
        public async Task<GetUserForUserDTO> UpdateUserAsync(int userId, UpdateUserInformationDTO dto)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null||user.IsActive==false)
                {
                    _logger.LogWarning("User not found with ID: {Id}", userId);
                    return null;
                }

                // Update basic properties
                user.UserName = dto.Name ?? user.UserName;//return left if not null and then right
                user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;
                user.City = dto.City ?? user.City;
                user.Area = dto.Area ?? user.Area;
                user.Street = dto.Street ?? user.Street;

                // Handle Img update
                if (dto.Img != null)
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

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                var updatedUser = new GetUserForUserDTO
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    ImgUrl = user.Img,
                    PhoneNumber = user.PhoneNumber,
                    City = user.City,
                    Area = user.Area,
                    Street = user.Street
                };

                _logger.LogInformation("User updated successfully with ID: {Id}", userId);
                return updatedUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the user with ID: {Id}", userId);
                return null;
            }
        }
    }
}
