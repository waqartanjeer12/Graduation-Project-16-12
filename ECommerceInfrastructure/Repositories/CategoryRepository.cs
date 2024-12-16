using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ECommerceCore.DTOs.Category;
using ECommerceCore.Models;
using ECommerceInfrastructure.Configurations.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerceInfrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryReadDTO>> GetAllCategoriesForUserAsync()
        {
            return await _context.Categories
                .Select(c => new CategoryReadDTO { Img = c.Img, Name = c.Name })
                .ToListAsync();
        }

        public async Task<List<Category>> GetAllCategoriesForAdminAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> CreateCategoryAsync(CategoryCreateUpdateDTO categoryDto)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "files", "images");
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}_{categoryDto.Img.FileName}";
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await categoryDto.Img.CopyToAsync(stream);
            }

            var category = new Category
            {
                Img = $"/files/images/{fileName}",
                Name = categoryDto.Name
            };

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> UpdateCategoryAsync(int id, CategoryCreateUpdateDTO categoryDto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            if (categoryDto.Img != null && categoryDto.Img.Length > 0)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "files", "images");
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                var fileName = $"{Guid.NewGuid()}_{categoryDto.Img.FileName}";
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await categoryDto.Img.CopyToAsync(stream);
                }
                category.Img = $"/files/images/{fileName}";
            }

            category.Name = categoryDto.Name;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<CategoryReadDTO>> SearchCategoriesByNameAsync(string name)
        {
            return await _context.Categories
                .Where(c => c.Name.Contains(name))
                .Select(c => new CategoryReadDTO { Img = c.Img, Name = c.Name })
                .ToListAsync();
        }
    }
}
