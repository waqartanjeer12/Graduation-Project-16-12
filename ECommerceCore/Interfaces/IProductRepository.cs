using ECommerceCore.DTOs.Color;
using ECommerceCore.DTOs.Product;
using ECommerceCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceCore.Interfaces
{
    public interface IProductRepository
    {

        Task<Color> CreateColorAsync(ColorCreateDTO colorCreateDTO);
        Task<List<ColorReadDTO>> GetColorAsync();
        Task DeleteColorAsync(int id);
        Task<ColorReadDTO> UpdateColorAsync(int id, ColorUpdateDTO colorUpdateDTO);

        Task<ProductReadForCreateDTO> CreateProductAsync(ProductCreateDTO productCreateDTO);
        Task<List<ProductReadForAdminDTO>> GetAllProductsForAdminAsync();
        Task<List<ProductReadForUserDTO>> GetAllProductsForUserAsync();
        Task<ProductReadByProductIdDTO> GetProductByProductIdAsync(int id);
        Task<List<ProductReadByCategoryIdDTO>> GetProductByCategoryIdAsync(int id);
        Task<ProductReadForUpdateDTO> UpdateProductAsync(int id, ProductUpdateDTO productUpdateDTO);
        Task<List<ProductReadByOriginalPrice>> GetAlProductsByComparisionOriginalPrice();
        Task<List<ProductReadForSearchDTO>> SearchProductsAsync(string query);
        Task<bool> DeleteProductAsync(int id);
    }
}