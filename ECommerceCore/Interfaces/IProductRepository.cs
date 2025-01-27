using ECommerceCore.DTOs.Color;
using ECommerceCore.DTOs.Product;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceCore.Interfaces
{
    public interface IProductRepository
    {
        Task<Dictionary<string, string[]>> CreateColorAsync(ColorCreateDTO colorCreateDTO);
        Task<List<ColorReadDTO>> GetColorAsync();
        Task<Dictionary<string, string[]>> DeleteColorAsync(int id);
        Task<ColorReadDTO> GetColorByIdAsync(int id);
        Task<Dictionary<string, string[]>> UpdateColorAsync(int id, ColorUpdateDTO colorUpdateDTO);

        Task<Dictionary<string, string[]>> CreateProductAsync(ProductCreateDTO productCreateDTO);
        Task<List<ProductReadForAdminDTO>> GetAllProductsForAdminAsync();
        Task<List<ProductReadForUserDTO>> GetAllProductsForUserAsync();
        Task<ProductReadByProductIdDTO> GetProductByProductIdAsync(int id);
        Task<List<ProductReadByCategoryIdDTO>> GetProductByCategoryIdAsync(int id);
        Task<Dictionary<string, string[]>> UpdateProductAsync(int id, ProductUpdateDTO productUpdateDTO);
        Task<List<ProductReadByOriginalPrice>> GetAlProductsByComparisionOriginalPrice();
        Task<List<ProductReadForSearchDTO>> SearchProductsAsync(string query);
        Task<Dictionary<string, string[]>> DeleteProductAsync(int id);
    }
}