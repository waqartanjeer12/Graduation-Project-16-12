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
        Task<List<CategoryReadForAdminDTO>> GetAllCategoriesForAdminAsync();
        Task<CategoryReadDTO> GetCategoryByIdAsync(int id);

        Task<Category> CreateCategoryAsync(CategoryCreateDTO categoryDto);
        Task<CategoryReadDTO> UpdateCategoryAsync(int id, CategoryUpdateDTO categoryDto); // Update an existing category
        Task<bool> DeleteCategoryAsync(int id);
        Task<List<CategoryReadDTO>> SearchCategoriesByNameAsync(string name);
        Task<CategoryReadForIdName> GetCategoryReadForIdName(int id);

    }
}

