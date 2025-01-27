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
        public async Task<Dictionary<string, string[]>> CreateColorAsync(ColorCreateDTO colorCreateDTO)
        {
            _logger.LogInformation("بداية إضافة لون جديد");
            var errors = new Dictionary<string, string[]>();

            try
            {
                if (colorCreateDTO == null)
                {
                    _logger.LogWarning("colorCreateDTO is null");
                    errors.Add("Color", new[] { "اللون المدخل غير موجود." });
                    return errors;
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
                return null; // No errors
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء اضافة اللون: نوع الملف غير مدعوم.");
                errors.Add("ColorImage", new[] { "يرجى تحميل صورة بتنسيق JPG أو PNG أو webp فقط." });
                return errors;
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تحميل ملف اللون.");
                errors.Add("ColorImage", new[] { "حدث خطأ أثناء تحميل ملف اللون." });
                return errors;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء اضافة اللون.");
                errors.Add("Color", new[] { "حدث خطأ أثناء اضافة صورة اللون." });
                return errors;
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
        public async Task<ColorReadDTO> GetColorByIdAsync(int id)
        {
            _logger.LogInformation("بداية جلب اللون باستخدام المعرف: {Id}", id);

            try
            {
                var color = await _context.Colors
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (color == null)
                {
                    _logger.LogWarning("لم يتم العثور على اللون بالمعرف: {Id}", id);
                    return null;
                }

                var colorReadDTO = new ColorReadDTO
                {
                    Id = color.Id,
                    Name = color.Name,
                    ColorImage = color.Image
                };

                _logger.LogInformation("تم جلب اللون بنجاح بالمعرف: {Id}", id);

                return colorReadDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء جلب اللون بالمعرف: {Id}", id);
                return null;
            }
        }
        public async Task<Dictionary<string, string[]>> UpdateColorAsync(int id, ColorUpdateDTO colorUpdateDTO)
        {
            _logger.LogInformation("بداية تحديث اللون ");
            var errors = new Dictionary<string, string[]>();

            try
            {
                var color = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);

                if (color == null)
                {
                    _logger.LogWarning("لم يتم العثور على اللون");
                    errors.Add("Color", new[] { "اللون المدخل غير موجود." });
                    return errors;
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

                await _context.SaveChangesAsync();

                _logger.LogInformation("تم تحديث اللون بنجاح ");
                return null; // No errors
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تحديث اللون: نوع الملف غير مدعوم.");
                errors.Add("ColorImage", new[] { "يرجى تحميل صورة بتنسيق JPG أو PNG أو webp فقط." });
                return errors;
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تحميل ملف اللون.");
                errors.Add("ColorImage", new[] { "حدث خطأ أثناء تحميل ملف اللون." });
                return errors;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تحديث اللون  ");
                errors.Add("Color", new[] { "حدث خطأ أثناء اضافة صورة اللون" });
                return errors;
            }
        }

        // Method to delete a color
        public async Task<Dictionary<string, string[]>> DeleteColorAsync(int id)
        {
            _logger.LogInformation("بداية حذف اللون ");
            var errors = new Dictionary<string, string[]>();

            try
            {
                var color = await _context.Colors
                    .Include(c => c.ProductColors)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (color == null)
                {
                    _logger.LogWarning("لم يتم العثور على اللون ");
                    errors.Add("Color", new[] { "Color not found." });
                    return errors;
                }

                // Check if the color is associated with any products
                if (color.ProductColors.Any())
                {
                    _logger.LogWarning("لا يمكن حذف اللون لأنه مرتبط بمنتجات ");
                    errors.Add("Color", new[] { "لا يمكن حذف اللون لأنه مرتبط بمنتجات" });
                    return errors;
                }

                _context.Colors.Remove(color);
                await _context.SaveChangesAsync();

                _logger.LogInformation("تم حذف اللون بنجاح ");
                return null; // No errors
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء حذف اللون ");
                errors.Add("Color", new[] { "An error occurred while deleting the color." });
                return errors;
            }
        }
        // Method to create a new product
        public async Task<Dictionary<string, string[]>> CreateProductAsync(ProductCreateDTO productCreateDTO)
        {
            _logger.LogInformation("بداية إضافة منتج جديد");
            var errors = new Dictionary<string, string[]>();

            try
            {
                if (productCreateDTO == null)
                {
                    _logger.LogWarning("ProductCreateDTO is null");
                    errors.Add("Product", new[] { "المنتج المدخل غير موجود" });
                    return errors;
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
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == productCreateDTO.CategoryName);
                if (category == null)
                {
                    _logger.LogWarning("الفئة غير موجودة يرجى البحث عن فئة أخرى", productCreateDTO.CategoryName);
                    errors.Add("CategoryName", new[] { "الفئة غير موجودة يرجى البحث عن فئة أخرى." });
                    return errors;
                }

                // Fetch existing colors based on provided names
                var productColors = new List<ProductColor>();
                var colorDetails = new List<ColorReadForUserDTO>();

                if (productCreateDTO.ColorNames != null && productCreateDTO.ColorNames.Any())
                {
                    foreach (var colorName in productCreateDTO.ColorNames)
                    {
                        var color = await _context.Colors.FirstOrDefaultAsync(c => c.Name == colorName);

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

                return null; // No errors
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء اضافة المنتج: نوع الملف غير مدعوم.");
                errors.Add("MainImage", new[] { "يرجى تحميل صورة بتنسيق JPG أو PNG أو webp فقط." });
                return errors;
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تحميل ملف المنتج.");
                errors.Add("MainImage", new[] { "حدث خطأ أثناء تحميل ملف المنتج." });
                return errors;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء اضافة المنتج.");
                errors.Add("Product", new[] { "حدث خطأ أثناء انشاء المنتج" });
                return errors;
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
        public async Task<Dictionary<string, string[]>> DeleteProductAsync(int id)
        {
            _logger.LogInformation("بداية حذف المنتج  ");
            var errors = new Dictionary<string, string[]>();

            try
            {
                // Retrieve the product along with its additional images
                var product = await _context.Products
                    .Include(p => p.AdditionalImages)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    _logger.LogWarning("لم يتم العثور على المنتج ");
                    errors.Add("Product", new[] { "المنتج المدخل غير موجود" });
                    return errors;
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
                return null; // No errors
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء حذف المنتج  ");
                errors.Add("General", new[] { "حدث خطأ أثناء حذف المنتج" });
                return errors;
            }
        }

        public async Task<Dictionary<string, string[]>> UpdateProductAsync(int id, ProductUpdateDTO productUpdateDTO)
        {
            _logger.LogInformation("بداية تحديث المنتج باستخدام المعرف: {Id}", id);
            var errors = new Dictionary<string, string[]>();

            try
            {
                var product = await _context.Products
                    .Include(p => p.AdditionalImages)
                    .Include(p => p.Colors)
                    .ThenInclude(pc => pc.Color)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    _logger.LogWarning("المنتج المدخل غير موجود");
                    errors.Add("Product", new[] { "المنتج المدخل غير موجود" });
                    return errors;
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
                if (productUpdateDTO.ColorNames != null && productUpdateDTO.ColorNames.Any())
                {
                    var existingColorIds = product.Colors.Select(pc => pc.ColorId).ToList();

                    foreach (var colorName in productUpdateDTO.ColorNames)
                    {
                        var color = await _context.Colors.FirstOrDefaultAsync(c => c.Name == colorName);

                        if (color != null && !existingColorIds.Contains(color.Id))
                        {
                            product.Colors.Add(new ProductColor { ProductId = product.Id, ColorId = color.Id });
                        }
                        else if (color == null)
                        {
                            _logger.LogWarning("Color not found: {ColorName}", colorName);
                        }
                    }
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("تم تحديث المنتج بنجاح: {Id}", id);
                return null; // No errors
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تحديث المنتج: نوع الملف غير مدعوم.");
                errors.Add("MainImage", new[] { "يرجى تحميل صورة بتنسيق JPG أو PNG أو webp فقط." });
                return errors;
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تحميل ملف المنتج.");
                errors.Add("MainImage", new[] { "حدث خطأ أثناء تحميل ملف المنتج." });
                return errors;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تحديث المنتج: {Id}", id);
                errors.Add("General", new[] { "An error occurred while updating the product." });
                return errors;
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