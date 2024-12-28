using ECommerceCore.DTOs.Color;
using ECommerceCore.DTOs.Product;
using ECommerceCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceCore.Interfaces
{
    public interface IProductRepository
    {
        Task<ProductReadForCreateDTO> CreateProductAsync(ProductCreateDTO productCreateDTO);
        Task<Color> CreateColorAsync(ColorCreateDTO colorCreateDTO);
        Task<List<ColorReadDTO>> GetColorAsync();
        Task<List<ProductReadForAdminDTO>> GetAllProductsForAdminAsync();
        Task<List<ProductReadForUserDTO>> GetAllProductsForUserAsync();
    }
}