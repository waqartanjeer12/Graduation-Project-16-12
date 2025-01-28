using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ECommerceCore.DTOs.Category;
using ECommerceCore.Models;
using ECommerceInfrastructure.Configurations.Data;
using ECommerceCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ECommerceInfrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;
        private readonly ILogger<CategoryRepository> _logger;

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
                    .FirstOrDefaultAsync();

                if (category == null)
                {
                    _logger.LogWarning("لم يتم العثور على أي فئة للمعرف: {Id}", id);
                }

                return category;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء جلب الفئة بالمعرف: {Id}", id);
                throw;
            }
        }

        public async Task<CategoryReadForIdName> GetCategoryReadForIdName(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return null;
            }
            return new CategoryReadForIdName
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task<Dictionary<string, string[]>> CreateCategoryAsync(CategoryCreateDTO categoryDto)
        {
            _logger.LogInformation("بداية إنشاء فئة جديدة");
            var errors = new Dictionary<string, string[]>();

            try
            {
                if (categoryDto == null)
                {
                    _logger.LogWarning("تم استدعاء CreateCategoryAsync مع DTO فارغ.");
                    errors.Add("General", new[] { "CreateCategoryAsync was called with a null DTO." });
                    return errors;
                }

                var existingCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Name == categoryDto.Name);

                if (existingCategory != null)
                {
                    _logger.LogWarning("اسم الفئة موجود بالفعل: {Name}", categoryDto.Name);
                    errors.Add("Name", new[] { "اسم الفئة موجود بالفعل. يرجى اختيار اسم آخر." });
                    return errors;
                }

                var fileName = await _fileService.UploadFileAsync(categoryDto.Img, "images");
                var category = new Category
                {
                    Img = $"/files/images/{fileName}",
                    Name = categoryDto.Name
                };

                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation("تم إنشاء الفئة بنجاح بالاسم: {Name}", category.Name);

                return null; // No errors
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء إنشاء الفئة: نوع الملف غير مدعوم.");
                errors.Add("Img", new[] { "يرجى تحميل صورة بتنسيق jpg  أو  jpeg  أو png  أو  webp فقط" });
                return errors;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء إنشاء الفئة.");
                errors.Add("General", new[] { "An error occurred while creating the category." });
                return errors;
            }
        }

        public async Task<Dictionary<string, string[]>> UpdateCategoryAsync(int id, CategoryUpdateDTO categoryDto)
        {
            _logger.LogInformation("البدء بالتعديل باستخدام المعرف: {Id}", id);
            var errors = new Dictionary<string, string[]>();

            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    _logger.LogWarning("الفئة غير موجودة بالنسبة للمعرف: {Id}", id);
                    errors.Add("category", new[] { "الفئة المدخلة غير موجودة" });
                    return errors;
                }

                if (!string.IsNullOrEmpty(categoryDto.Name) && category.Name != categoryDto.Name)
                {
                    var existingCategory = await _context.Categories
                        .FirstOrDefaultAsync(c => c.Name == categoryDto.Name && c.Id != id);

                    if (existingCategory != null)
                    {
                        _logger.LogWarning("اسم الفئة موجود بالفعل: {Name}", categoryDto.Name);
                        errors.Add("Name", new[] { "اسم الفئة موجود بالفعل. يرجى اختيار اسم آخر." });
                        return errors;
                    }

                    category.Name = categoryDto.Name;
                }

                if (categoryDto.Img != null && categoryDto.Img.Length > 0)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(category.Img))
                        {
                            _fileService.DeleteFile(Path.GetFileName(category.Img), "images");
                            _logger.LogInformation("حذف الصورة القديمة حسب المعرف: {Id}", id);
                        }

                        var fileName = await _fileService.UploadFileAsync(categoryDto.Img, "images");
                        category.Img = $"/files/images/{fileName}";
                    }
                    catch (InvalidOperationException ex)
                    {
                        _logger.LogError(ex, "حدث خطأ أثناء تحديث الفئة: نوع الملف غير مدعوم.");
                        errors.Add("Img", new[] { "يرجى تحميل صورة بتنسيق jpg  أو  jpeg  أو png  أو  webp فقط" });
                        return errors;
                    }
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("تم التعديل على الفئة بنجاح: {Id}", id);

                return null; // No errors
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء العديل على الفئة: {Id}", id);
                errors.Add("category", new[] { "حدث خطأ أثناء التعديل على الفئة" });
                return errors;
            }
        }

        public async Task<Dictionary<string, string[]>> DeleteCategoryAsync(int id)
        {
            _logger.LogInformation("بداية حذف الفئة للـ ID: {Id}", id);
            var errors = new Dictionary<string, string[]>();

            try
            {
                var category = await _context.Categories
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    _logger.LogWarning("لم يتم العثور على فئة بالـ ID: {Id}", id);
                    errors.Add("category", new[] { "الفئة المدخلة غير موجودة" });
                    return errors;
                }

                if (category.Products.Any())
                {
                    _logger.LogWarning("لا يمكن حذف الفئة لأن هناك منتجات مرتبطة بها: {Id}", id);
                    errors.Add("category", new[] { "لا يمكن حذف الفئة لأن هناك منتجات مرتبطة بها." });
                    return errors;
                }

                if (!string.IsNullOrEmpty(category.Img))
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "files", "images", Path.GetFileName(category.Img));
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        _logger.LogInformation("تم حذف الصورة الخاصة بالفئة ID: {Id}", id);
                    }
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation("تم حذف الفئة بنجاح للـ ID: {Id}", id);

                return null; // No errors
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء حذف الفئة للـ ID: {Id}", id);
                errors.Add("General", new[] { "An error occurred while deleting the category." });
                return errors;
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
                return new List<CategoryReadDTO>();
            }
        }
    }
}