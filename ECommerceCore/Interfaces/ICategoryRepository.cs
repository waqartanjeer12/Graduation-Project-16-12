using ECommerceCore.DTOs.Category;
using ECommerceCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    

namespace ECommerceInfrastructure.Repositories
    {
        public interface ICategoryRepository
        {
            Task<List<CategoryReadDTO>> GetAllCategoriesForUserAsync();
            Task<List<Category>> GetAllCategoriesForAdminAsync();
            Task<Category> CreateCategoryAsync(CategoryCreateUpdateDTO categoryDto);
            Task<bool> UpdateCategoryAsync(int id, CategoryCreateUpdateDTO categoryDto);
            Task<bool> DeleteCategoryAsync(int id);
            Task<List<CategoryReadDTO>> SearchCategoriesByNameAsync(string name);
        }
    }

