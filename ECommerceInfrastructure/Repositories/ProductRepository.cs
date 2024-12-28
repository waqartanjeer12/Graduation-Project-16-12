using ECommerceCore.DTOs.Color;
using ECommerceCore.DTOs.Product;
using ECommerceCore.Interfaces;
using ECommerceCore.Models;
using ECommerceInfrastructure.Configurations.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceInfrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(ApplicationDbContext context, IFileService fileService, ILogger<ProductRepository> logger)
        {
            _context = context;
            _fileService = fileService;
            _logger = logger;
        }

        // Method to create a new color
        public async Task<Color> CreateColorAsync(ColorCreateDTO colorCreateDTO)
        {
            _logger.LogInformation("بداية إنشاء فئة جديدة");

            try
            {
                if (colorCreateDTO == null)
                {
                    _logger.LogWarning("colorCreateDTO is null");
                    return null;
                }

                var fileName = await _fileService.UploadFileAsync(colorCreateDTO.ColorImage, "images");
                var colors = new Color
                {
                    Image = $"/files/images/{fileName}",
                    Name = colorCreateDTO.Name
                };

                await _context.Colors.AddAsync(colors);
                await _context.SaveChangesAsync();

                _logger.LogInformation("تم اضافة اللون بنجاح بالاسم: {Name}", colors.Name);

                return colors;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء اضافة اللون.");
                return null;
            }
        }

        // Method to get all colors
        public async Task<List<ColorReadDTO>> GetColorAsync()
        {
            return await _context.Colors
                .Select(c => new ColorReadDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    ColorImage = c.Image
                })
                .ToListAsync();
        }

        // Method to create a new product
        public async Task<ProductReadForCreateDTO> CreateProductAsync(ProductCreateDTO productCreateDTO)
        {
            _logger.LogInformation("بداية إنشاء منتج جديد");

            try
            {
                if (productCreateDTO == null)
                {
                    _logger.LogWarning("ProductCreateDTO is null");
                    return null;
                }

                // Upload main image and get URL
                var mainImageFileName = await _fileService.UploadFileAsync(productCreateDTO.MainImage, "images");
                var mainImageUrl = $"/files/images/{mainImageFileName}";

                // Upload additional images and get URLs
                var additionalImageFiles = new List<ProductImage>();
                var additionalImageUrls = new List<string>();
                foreach (var additionalImage in productCreateDTO.AdditionalImages)
                {
                    var fileName = await _fileService.UploadFileAsync(additionalImage, "images");
                    additionalImageFiles.Add(new ProductImage
                    {
                        ImagePath = $"/files/images/{fileName}"
                    });
                    additionalImageUrls.Add($"/files/images/{fileName}");
                }

                // Retrieve the category by name
                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Name == productCreateDTO.CategoryName);
                if (category == null)
                {
                    _logger.LogWarning("الفئة غير موجودة يرجى البحث عن فئة أخرى", productCreateDTO.CategoryName);
                    return null;
                }

                // Fetch existing colors based on provided names
                var productColors = new List<ProductColor>();
                var colorDetails = new List<ColorReadForUserDTO>();

                if (productCreateDTO.ColorNames != null && productCreateDTO.ColorNames.Any())
                {
                    foreach (var colorName in productCreateDTO.ColorNames)
                    {
                        var color = await _context.Colors
                            .FirstOrDefaultAsync(c => c.Name == colorName);

                        if (color != null)
                        {
                            productColors.Add(new ProductColor { ColorId = color.Id });
                            colorDetails.Add(new ColorReadForUserDTO { Name = color.Name, ColorImage = color.Image });
                        }
                        else
                        {
                            _logger.LogWarning("اللون غير موجود: {ColorName}", colorName);
                        }
                    }
                }

                // Create the product entity
                var product = new Product
                {
                    Name = productCreateDTO.Name,
                    Description = productCreateDTO.Description,
                    MainImage = mainImageUrl,
                    Inventory = productCreateDTO.Inventory,
                    CategoryId = category.Id,
                    AdditionalImages = additionalImageFiles,
                    Colors = productColors,
                    Price = productCreateDTO.Price,
                    OriginalPrice = productCreateDTO.OriginalPrice
                };

                // Save the product to the database
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("تم انشاء المنتج بنجاح : {Name}", product.Name);

                // Convert the saved product entity to ProductReadForUserDTO with URLs
                var productReadForCreateDTO = new ProductReadForCreateDTO
                {
                    Name = product.Name,
                    Description = product.Description,
                    CategoryName = category.Name,
                    MainImageUrl = mainImageUrl,
                    AdditionalImageUrls = additionalImageUrls,
                    Inventory = product.Inventory,
                    ColorDetails = colorDetails,
                    Price = productCreateDTO.Price,
                    OriginalPrice = productCreateDTO.OriginalPrice
                };

                return productReadForCreateDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء انشاء المنتج");
                throw;
            }
        }

        // Method to get all products for admin
        public async Task<List<ProductReadForAdminDTO>> GetAllProductsForAdminAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Colors)
                .ThenInclude(pc => pc.Color)
                .Select(p => new ProductReadForAdminDTO
                {
                    Id = p.Id,
                    MainImage = p.MainImage,
                    ProductName = p.Name,
                    CategoryName = p.Category.Name,
                    Description = p.Description,
                    Price = p.Price,
                    OriginalPrice = p.OriginalPrice,
                    Inventory = p.Inventory,
                    Colors = p.Colors.Select(c => new ColorReadForDisplayDTO
                    {
                        ColorImage = c.Color.Image
                    }).ToList()
                })
                .ToListAsync();
        }



        public async Task<List<ProductReadForUserDTO>> GetAllProductsForUserAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Colors)
                .ThenInclude(pc => pc.Color)
                .Include(p => p.AdditionalImages)
                .Select(p => new ProductReadForUserDTO
                {
                    Name = p.Name,
                    Description = p.Description,
                    CategoryName = p.Category.Name,
                    MainImageUrl = p.MainImage,
                    AdditionalImageUrls = p.AdditionalImages.Select(ai => ai.ImagePath).ToList(),
                    Price = p.Price,
                    OriginalPrice = p.OriginalPrice,
 
                    ColorDetails = p.Colors.Select(c => new ColorReadForUserDTO
                    {
                        Name = c.Color.Name,
                        ColorImage = c.Color.Image
                    }).ToList()
                })
                .ToListAsync();
        }
    }
}