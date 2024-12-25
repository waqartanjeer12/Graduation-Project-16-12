using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ECommerceCore.DTOs.Category;
using ECommerceCore.Models;
using ECommerceInfrastructure.Configurations.Data;
using ECommerceCore.Interfaces;  // Add this for IFileService
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;  // Add this for ILogger

namespace ECommerceInfrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;  // Inject IFileService
        private readonly ILogger<CategoryRepository> _logger;  // Inject ILogger

        public CategoryRepository(ApplicationDbContext context, IFileService fileService, ILogger<CategoryRepository> logger)
        {
            _context = context;
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<List<CategoryReadDTO>> GetAllCategoriesForUserAsync()
        {
            return await _context.Categories
                .Select(c => new CategoryReadDTO { Img = c.Img, Name = c.Name })
                .ToListAsync();
        }
        public async Task<List<CategoryReadForAdminDTO>> GetAllCategoriesForAdminAsync()
        {
            return await _context.Categories
                .Select(c => new CategoryReadForAdminDTO
                {
                    Id = c.Id,
                    Img = c.Img,
                    Name = c.Name
                })
                .ToListAsync();
        }


        public async Task<CategoryReadDTO> GetCategoryByIdAsync(int id)
{
    _logger.LogInformation("جارِ جلب الفئة بالمعرف: {Id}", id);

    try
    {
                var category = await _context.Categories
            .Where(c => c.Id == id)
            .Select(c => new CategoryReadDTO
            {
                Img = c.Img,
                Name = c.Name
            })
            .FirstOrDefaultAsync(); // Fetch a single category


                if (category == null)
        {
            _logger.LogWarning("لم يتم العثور على أي فئة للمعرف: {Id}", id);
        }

        return category;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "حدث خطأ أثناء جلب الفئة بالمعرف: {Id}", id);
        throw; // Rethrow the exception to be handled by the controller
    }
}




        public async Task<Category> CreateCategoryAsync(CategoryCreateDTO categoryDto)
        {
            _logger.LogInformation("بداية إنشاء فئة جديدة");

            try
            {
                if (categoryDto == null)
                {
                    _logger.LogWarning("تم استدعاء CreateCategoryAsync مع DTO فارغ.");
                    return null;  // Return null or a specific failure value
                }

                var fileName = await _fileService.SaveFileAsync(categoryDto.Img, "images");
                var category = new Category
                {
                    Img = $"/files/images/{fileName}",
                    Name = categoryDto.Name
                };

                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation("تم إنشاء الفئة بنجاح بالاسم: {Name}", category.Name);

                return category;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء إنشاء الفئة.");
                return null;  // Return null or a specific failure value
            }
        }

        public async Task<CategoryReadDTO> UpdateCategoryAsync(int id, CategoryUpdateDTO categoryDto)
        {
            _logger.LogInformation("بدء عملية تحديث الفئة للـ ID: {Id}", id);

            try
            {
                // العثور على الفئة حسب ID
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    _logger.LogWarning("لم يتم العثور على الفئة للـ ID: {Id}", id);
                    return null; // لم يتم العثور على الفئة
                }

                // تحقق إذا تم تعديل الاسم
                if (!string.IsNullOrEmpty(categoryDto.Name) && category.Name != categoryDto.Name)
                {
                    _logger.LogInformation("تم تحديث الاسم للـ ID: {Id}", id);
                    category.Name = categoryDto.Name;
                }

                // تحقق إذا تم تعديل الصورة
                if (categoryDto.Img != null && categoryDto.Img.Length > 0)
                {
                    // حذف الصورة القديمة إن وجدت
                    if (!string.IsNullOrEmpty(category.Img))
                    {
                        _fileService.DeleteFile(Path.GetFileName(category.Img), "images");
                        _logger.LogInformation("تم حذف الصورة القديمة للـ ID: {Id}", id);
                    }

                    // حفظ الصورة الجديدة
                    var fileName = await _fileService.SaveFileAsync(categoryDto.Img, "images");
                    category.Img = $"/files/images/{fileName}";
                    _logger.LogInformation("تم تحديث الصورة للـ ID: {Id}", id);
                }
                else if (!string.IsNullOrEmpty(categoryDto.ImgUrl) && category.Img != categoryDto.ImgUrl)
                {
                    _logger.LogInformation("تم تحديث الصورة باستخدام رابط للـ ID: {Id}", id);
                    category.Img = categoryDto.ImgUrl;
                }

                // حفظ التغييرات في قاعدة البيانات
                await _context.SaveChangesAsync();
                _logger.LogInformation("تم تحديث الفئة بنجاح للـ ID: {Id}", id);

                // Map the updated category entity to CategoryReadDTO
                var categoryReadDto = new CategoryReadDTO
                {
                    Img = category.Img,
                    Name = category.Name
                };

                // إرجاع البيانات المحدثة كـ CategoryReadDTO
                return categoryReadDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تحديث الفئة للـ ID: {Id}", id);
                return null; // حدث خطأ أثناء التحديث
            }
        }






        public async Task<bool> DeleteCategoryAsync(int id)
        {
            _logger.LogInformation("بداية حذف الفئة للـ ID: {Id}", id);

            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    _logger.LogWarning("لم يتم العثور على فئة بالـ ID: {Id}", id);
                    return false;
                }

                if (!string.IsNullOrEmpty(category.Img))
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "files", "images", Path.GetFileName(category.Img));
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);  // Delete the file
                        _logger.LogInformation("تم حذف الصورة الخاصة بالفئة ID: {Id}", id);
                    }
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation("تم حذف الفئة بنجاح للـ ID: {Id}", id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء حذف الفئة للـ ID: {Id}", id);
                return false;  // Return false if the deletion fails
            }
        }

        public async Task<List<CategoryReadDTO>> SearchCategoriesByNameAsync(string name)
        {
            _logger.LogInformation("البحث عن الفئات حسب الاسم: {Name}", name);

            try
            {
                var results = await _context.Categories
                    .Where(c => c.Name.Contains(name))
                    .Select(c => new CategoryReadDTO { Img = c.Img, Name = c.Name })
                    .ToListAsync();

                _logger.LogInformation("تم العثور على {Count} فئة مطابقة للاسم: {Name}", results.Count, name);

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء البحث عن الفئات حسب الاسم: {Name}", name);
                return new List<CategoryReadDTO>();  // Return an empty list if an error occurs
            }
        }
    }
}

