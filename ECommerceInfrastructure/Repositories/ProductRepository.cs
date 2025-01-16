using ECommerceCore.DTOs.Color;
using ECommerceCore.DTOs.Product;
using ECommerceCore.Interfaces;
using ECommerceCore.Models;
using ECommerceInfrastructure.Configurations.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
            _logger.LogInformation("بداية إضافة لون جديد");

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

                _logger.LogInformation("تم اضافة اللون بنجاح ");

                return colors;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء اضافة اللون.");
                return null;
            }
        }

        public async Task<List<ColorReadDTO>> GetColorAsync()
        {
            try
            {
                _logger.LogInformation("جارِ جلب جميع الألوان.");

                var colors = await _context.Colors
                    .Select(c => new ColorReadDTO
                    {
                        Id = c.Id,
                        ColorImage = c.Image,
                        Name = c.Name

                    })
                    .ToListAsync();

                _logger.LogInformation("تم جلب جميع الألوان بنجاح.");
                return colors;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء جلب الألوان.");
                return new List<ColorReadDTO>(); // إرجاع قائمة فارغة في حالة حدوث خطأ
            }
        }

        public async Task<ColorReadDTO> UpdateColorAsync(int id, ColorUpdateDTO colorUpdateDTO)
        {
            _logger.LogInformation("بداية تحديث اللون ");

            try
            {
                var color = await _context.Colors
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (color == null)
                {
                    _logger.LogWarning("لم يتم العثور على اللون");
                    return null;
                }

                if (colorUpdateDTO.ColorImage != null)
                {
                    var fileName = await _fileService.UploadFileAsync(colorUpdateDTO.ColorImage, "images");
                    color.Image = $"/files/images/{fileName}";
                }
                if (colorUpdateDTO.Name != null)
                {

                    color.Name = colorUpdateDTO.Name;
                }
                ColorReadDTO colorReadDTO = new ColorReadDTO
                {
                    Id = color.Id,
                    Name = color.Name,
                    ColorImage = color.Image
                };
                await _context.SaveChangesAsync();

                _logger.LogInformation("تم تحديث اللون بنجاح ");
                return colorReadDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تحديث اللون  ");
                return null;
            }
        }

        // Method to delete a color
        public async Task DeleteColorAsync(int id)
        {
            _logger.LogInformation("بداية حذف اللون  ");

            try
            {
                var color = await _context.Colors
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (color == null)
                {
                    _logger.LogWarning("لم يتم العثور على اللون ");
                    return;
                }

                _context.Colors.Remove(color);
                await _context.SaveChangesAsync();

                _logger.LogInformation("تم حذف اللون بنجاح  ");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء حذف اللون  ");
            }
        }
        // Method to create a new product
        public async Task<ProductReadForCreateDTO> CreateProductAsync(ProductCreateDTO productCreateDTO)
        {
            _logger.LogInformation("بداية إضافة منتج جديد");

            try
            {
                if (productCreateDTO == null)
                {
                    _logger.LogWarning("ProductCreateDTO is null");
                    return null;
                }

                // Upload the main image and get URL
                var mainImageFileName = await _fileService.UploadFileAsync(productCreateDTO.MainImage, "images");
                var mainImageUrl = $"/files/images/{mainImageFileName}";

                // Upload additional images and get URLs
                var additionalImageFiles = new List<ProductImage>();
                var additionalImageUrls = new List<string>();
                if (productCreateDTO.AdditionalImages != null)
                {
                    foreach (var additionalImage in productCreateDTO.AdditionalImages)
                    {
                        var fileName = await _fileService.UploadFileAsync(additionalImage, "images");
                        additionalImageFiles.Add(new ProductImage
                        {
                            ImageUrl = $"/files/images/{fileName}"
                        });
                        additionalImageUrls.Add($"/files/images/{fileName}");
                    }
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
                            _logger.LogWarning("يرجى إدخال اسم اللون: {ColorName}", colorName);
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

                _logger.LogInformation("تم إضافة المنتج بنجاح : {Name}", product.Name);

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
                _logger.LogError(ex, "حدث خطأ أثناء اضافة المنتج");
                throw;
            }
        }
        // Method to get all products for admin
        public async Task<List<ProductReadForUserDTO>> GetAllProductsForUserAsync()
        {
            try
            {
                _logger.LogInformation("بداية جلب منتجات المستخدم");

                var products = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Colors)
                    .ThenInclude(pc => pc.Color)
                    .Include(p => p.AdditionalImages)
                    .Select(p => new ProductReadForUserDTO
                    {
                        ProductId = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        CategoryName = p.Category.Name,
                        MainImageUrl = p.MainImage,
                        AdditionalImageUrls = p.AdditionalImages.Select(ai => ai.ImageUrl).ToList(),
                        Price = p.Price,
                        OriginalPrice = p.OriginalPrice,
                        ColorDetails = p.Colors.Select(c => new ColorReadForUserDTO
                        {
                            Name = c.Color.Name,
                            ColorImage = c.Color.Image
                        }).ToList()
                    })
                    .ToListAsync();

                _logger.LogInformation("تم جلب المنتجات للمستخدم بنجاح.");
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء جلب المنتجات للمستخدم");
                return new List<ProductReadForUserDTO>(); // Return an empty list in case of error
            }
        }
        public async Task<List<ProductReadForAdminDTO>> GetAllProductsForAdminAsync()
        {
            try
            {
                _logger.LogInformation("بداية جلب المنتجات للأدمن");

                var products = await _context.Products
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

                _logger.LogInformation("تم جلب منتجات الأدمن بنجاح");
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء جلب منتجات الأدمن");
                return new List<ProductReadForAdminDTO>(); // Return an empty list in case of error
            }
        }
        public async Task<List<ProductReadByOriginalPrice>> GetAlProductsByComparisionOriginalPrice()
        {
            _logger.LogInformation("بداية احضار المنتجات التي عليها عرض");
            try
            {
                var products = await _context.Products
                    .Where(p => p.Price < p.OriginalPrice)
                    .Select(p => new ProductReadByOriginalPrice
                    {
                        ProductId=p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        CategoryName = p.Category.Name,
                        MainImageUrl = p.MainImage,
                        AdditionalImageUrls = p.AdditionalImages.Select(ai => ai.ImageUrl).ToList(),
                        Price = p.Price,
                        OriginalPrice = p.OriginalPrice,
                        ColorDetails = p.Colors.Select(c => new ColorReadForUserDTO
                        {
                           
                            Name = c.Color.Name,
                            ColorImage = c.Color.Image
                        }).ToList()
                    })
                    .ToListAsync();
                return products;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء جلب المنتجات التي عليها عرض");
                return new List<ProductReadByOriginalPrice>();
            }
        }
        public async Task<bool> DeleteProductAsync(int id)
        {
            _logger.LogInformation("بداية حذف المنتج  ");

            try
            {
                // Retrieve the product along with its additional images
                var product = await _context.Products
                    .Include(p => p.AdditionalImages)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    _logger.LogWarning("لم يتم العثور على المنتج ");
                    return false;
                }

                // Delete MainImage if it exists
                if (!string.IsNullOrEmpty(product.MainImage))
                {
                    var mainImagePath = Path.Combine(Directory.GetCurrentDirectory(), "files", "images", Path.GetFileName(product.MainImage));
                    if (File.Exists(mainImagePath))
                    {
                        File.Delete(mainImagePath);
                        _logger.LogInformation("تم حذف الصورة الرئيسية للمنتج ");
                    }
                }

                // Delete AdditionalImages if they exist
                foreach (var additionalImage in product.AdditionalImages)
                {
                    var additionalImagePath = Path.Combine(Directory.GetCurrentDirectory(), "files", "images", Path.GetFileName(additionalImage.ImageUrl));
                    if (File.Exists(additionalImagePath))
                    {
                        File.Delete(additionalImagePath);
                        _logger.LogInformation("تم حذف الصور الإضافية للمنتج ");
                    }
                }

                // Remove the product from the database
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("تم حذف المنتج بنجاح  ");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء حذف المنتج  ");
                return false;
            }
        }

        public async Task<ProductReadForUpdateDTO> UpdateProductAsync(int id, ProductUpdateDTO productUpdateDTO)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.AdditionalImages)
                    .Include(p => p.Colors)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    _logger.LogWarning("Product not found");
                    return null;
                }

                // Update basic properties
                if (!string.IsNullOrEmpty(productUpdateDTO.Name))
                {
                    product.Name = productUpdateDTO.Name;
                }

                product.Description = productUpdateDTO.Description;

                if (!string.IsNullOrEmpty(productUpdateDTO.CategoryName))
                {
                    var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == productUpdateDTO.CategoryName);
                    if (category != null)
                        product.CategoryId = category.Id;
                }

                if (productUpdateDTO.Price.HasValue)
                {
                    product.Price = productUpdateDTO.Price.Value;
                }

                product.OriginalPrice = productUpdateDTO.OriginalPrice;

                if (productUpdateDTO.Inventory.HasValue)
                {
                    product.Inventory = productUpdateDTO.Inventory.Value;
                }

                // Handle MainImage update
                if (productUpdateDTO.MainImage != null)
                {
                    var mainImageFileName = await _fileService.UploadFileAsync(productUpdateDTO.MainImage, "images");
                    product.MainImage = $"/files/images/{mainImageFileName}";
                }

                // Handle AdditionalImages update
                if (productUpdateDTO.AdditionalImageUrlsToDelete != null)
                {
                    foreach (var imageUrl in productUpdateDTO.AdditionalImageUrlsToDelete)
                    {
                        var image = product.AdditionalImages.FirstOrDefault(ai => ai.ImageUrl == imageUrl);
                        if (image != null)
                        {
                            _fileService.DeleteFile(Path.GetFileName(image.ImageUrl), "images");
                            product.AdditionalImages.Remove(image);
                        }
                    }
                }

                if (productUpdateDTO.AdditionalImages != null && productUpdateDTO.AdditionalImages.Any())
                {
                    foreach (var additionalImage in productUpdateDTO.AdditionalImages)
                    {
                        var fileName = await _fileService.UploadFileAsync(additionalImage, "images");
                        product.AdditionalImages.Add(new ProductImage { ImageUrl = $"/files/images/{fileName}" });
                    }
                }

                // Handle Colors update
                var productColors = new List<ProductColor>();
                var colorDetails = new List<ColorReadDTO>();
                if (productUpdateDTO.ColorNames != null && productUpdateDTO.ColorNames.Any())
                {
                    foreach (var colorName in productUpdateDTO.ColorNames)
                    {
                        var color = await _context.Colors
                            .FirstOrDefaultAsync(c => c.Name == colorName);

                        if (color != null)
                        {
                            productColors.Add(new ProductColor { ColorId = color.Id });
                            colorDetails.Add(new ColorReadDTO { Id = color.Id, Name = color.Name, ColorImage = color.Image });
                        }
                        else
                        {
                            _logger.LogWarning("Color not found");
                        }
                    }
                }

                await _context.SaveChangesAsync();

                // Map to ProductReadForUpdateDTO
                var productReadForUpdateDTO = new ProductReadForUpdateDTO
                {
                    Name = product.Name,
                    Description = product.Description,
                    CategoryName = (await _context.Categories.FindAsync(product.CategoryId))?.Name,
                    MainImageUrl = product.MainImage,
                    AdditionalImageUrls = product.AdditionalImages.Select(ai => ai.ImageUrl).ToList(),
                    Price = product.Price,
                    OriginalPrice = product.OriginalPrice,
                    Inventory = product.Inventory,
                    ColorDetails = colorDetails
                };

                _logger.LogInformation("Product updated successfully");
                return productReadForUpdateDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the product");
                return null;
            }
        }

        public async Task<ProductReadByProductIdDTO> GetProductByProductIdAsync(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Colors)
                    .ThenInclude(pc => pc.Color)
                    .Include(p => p.AdditionalImages)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    _logger.LogWarning("Product with ID: {Id} not found", id);
                    return null;
                }

                var colorDetails = product.Colors.Select(c => new ColorReadDTO
                {
                    Id = c.Color.Id,
                    Name = c.Color.Name,
                    ColorImage = c.Color.Image
                }).ToList();

                var productReadByProductIdDTO = new ProductReadByProductIdDTO
                {
                    Name = product.Name,
                    Description = product.Description,
                    CategoryName = product.Category.Name,
                    MainImageUrl = product.MainImage,
                    AdditionalImageUrls = product.AdditionalImages.Select(ai => ai.ImageUrl).ToList(),
                    Price = product.Price,
                    OriginalPrice = product.OriginalPrice,
                    Inventory = product.Inventory,
                    ColorDetails = colorDetails
                };

                return productReadByProductIdDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[حدث خطأ أثناء البحث عن المنتج باستخدام]: {Id}", id);
                return null;
            }
        }
        public async Task<List<ProductReadByCategoryIdDTO>> GetProductByCategoryIdAsync(int id)
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Colors)
                    .ThenInclude(pc => pc.Color)
                    .Include(p => p.AdditionalImages)
                    .Where(p => p.CategoryId == id)
                    .Select(p => new ProductReadByCategoryIdDTO
                    {
                        ProductId = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        CategoryName = p.Category.Name,
                        MainImageUrl = p.MainImage,
                        AdditionalImageUrls = p.AdditionalImages.Select(ai => ai.ImageUrl).ToList(),
                        Price = p.Price,
                        OriginalPrice = p.OriginalPrice,
                        Inventory = p.Inventory,
                        ColorDetails = p.Colors.Select(c => new ColorReadDTO
                        {
                            Id = c.Color.Id,
                            Name = c.Color.Name,
                            ColorImage = c.Color.Image
                        }).ToList()
                    })
                    .ToListAsync();

                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء البحث عن المنتجات باستخدام الفئة");
                return null;
            }
        }

        public async Task<List<ProductReadForSearchDTO>> SearchProductsAsync(string query)
        {
            // Split the query into words using spaces
            var words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // Search for products whose name or description contains any of the words
            var products = await _context.Products
                .Where(p => words.Any(word => p.Name.Contains(word)) || words.Any(word => p.Description.Contains(word)))
                .Select(p => new ProductReadForSearchDTO
                {
                    Name = p.Name,
                    Description = p.Description,
                    CategoryName = p.Category.Name,
                    MainImageUrl = p.MainImage,
                    AdditionalImageUrls = p.AdditionalImages.Select(ai => ai.ImageUrl).ToList(),
                    Price = p.Price,
                    OriginalPrice = p.OriginalPrice,
                    ColorDetails = p.Colors.Select(c => new ColorReadForUserDTO
                    {
                        Name = c.Color.Name,
                        ColorImage = c.Color.Image
                    }).ToList()
                })
                .ToListAsync();

            return products;
        }
        
        

    }

}