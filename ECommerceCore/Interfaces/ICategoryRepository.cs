using ECommerceCore.DTOs.Category;
using ECommerceCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceInfrastructure.Repositories
{
    public interface ICategoryRepository
    {
        Task<List<CategoryReadDTO>> GetAllCategoriesForUserAsync();
        Task<List<CategoryReadForAdminDTO>> GetAllCategoriesForAdminAsync();
        Task<CategoryReadDTO> GetCategoryByIdAsync(int id);
        Task<CategoryReadForIdName> GetCategoryReadForIdName(int id);

        Task<Dictionary<string, string[]>> CreateCategoryAsync(CategoryCreateDTO categoryDto);
        Task<Dictionary<string, string[]>> UpdateCategoryAsync(int id, CategoryUpdateDTO categoryDto);
        Task<Dictionary<string, string[]>> DeleteCategoryAsync(int id);

        Task<List<CategoryReadDTO>> SearchCategoriesByNameAsync(string name);
    }
}