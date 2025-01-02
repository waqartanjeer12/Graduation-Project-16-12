using ECommerceCore.DTOs.Color;
using ECommerceCore.DTOs.Product;
using ECommerceCore.Interfaces;
using ECommerceInfrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _repository;

        public ProductController(IProductRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("Color")]
        public async Task<IActionResult> GetColorAsync()
        {
            try
            {
                Log.Information("جارِ جلب جميع الألوان");

                // Retrieve the colors as ColorReadDTO
                var colors = await _repository.GetColorAsync();

                return Ok(colors);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "حدث خطأ أثناء جلب الألوان.");
                return StatusCode(500, "حدث خطأ أثناء جلب الألوان.");
            }
        }

        [HttpPost("Color")]
        public async Task<IActionResult> CreateColorAsync([FromForm] ColorCreateDTO colorDto)
        {
            try
            {
                var color = await _repository.CreateColorAsync(colorDto);
                return Ok(new { Message = "تم إنشاء اللون بنجاح", ColorId = color.Id });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "حدث خطأ أثناء اضافة اللون.");
                return StatusCode(500, "حدث خطأ أثناء اضافة اللون.");
            }
        }

        [HttpDelete("Color/{id}")]
        public async Task<IActionResult> DeleteColorAsync(int id)
        {
            try
            {
                await _repository.DeleteColorAsync(id);
                return StatusCode(204, new { Message = "تم حذف المنتج بنجاح." }); // 204 No Content
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[حدث خطأ أثناء حذف المنتج باستخدام المعرف]: {Id}", id);
                return StatusCode(404, "حدث خطأ أثناء حذف المنتج باستخدام المعرف"); // 500 Internal Server Error
            }
        }

        [HttpPut("Color/{id}")]
        public async Task<IActionResult> UpdateColorAsync([FromRoute] int id, [FromForm] ColorUpdateDTO colorDto)
        {
            try
            {
                var color = await _repository.UpdateColorAsync(id, colorDto);
                return Ok(new { Message = "تم تحديث اللون بنجاح", ColorId = color.Id });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[حدث خطأ أثناء تحديث اللون باستخدام المعرف]: {Id}", id);
                return StatusCode(500, "Internal server error"); // 500 Internal Server Error
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateProductAsync([FromForm] ProductCreateDTO productCreateDTO)
        {
            try
            {
                var createdProductDTO = await _repository.CreateProductAsync(productCreateDTO);
                return Ok(new { Message = "تم انشاء المنتج بنجاح ", Product = createdProductDTO });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "حدث خطأ أثناء انشاء المنتج.");
                return StatusCode(500, "حدث خطأ أثناء انشاء المنتج.");
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateProductAsync(int id, [FromForm] ProductUpdateDTO productUpdateDTO)
        {
            try
            {
                var updatedProductDTO = await _repository.UpdateProductAsync(id, productUpdateDTO);
                return Ok(new { Message = "تم تحديث المنتج بنجاح ", Product = updatedProductDTO });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "حدث خطأ أثناء تحديث المنتج.");
                return StatusCode(500, "حدث خطأ أثناء تحديث المنتج.");
            }
        }

        [HttpGet("admin")]
        public async Task<ActionResult<List<ProductReadForAdminDTO>>> GetAllProductsForAdmin()
        {
            try
            {
                var products = await _repository.GetAllProductsForAdminAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "حدث خطأ أثناء جلب المنتجات.");
                return StatusCode(500, "حدث خطأ أثناء جلب المنتجات.");
            }
        }

        [HttpGet("user")]
        public async Task<ActionResult<List<ProductReadForUserDTO>>> GetAllProductsForUser()
        {
            try
            {
                var products = await _repository.GetAllProductsForUserAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "حدث خطأ أثناء جلب المنتجات.");
                return StatusCode(500, "حدث خطأ أثناء جلب المنتجات.");
            }
        }
        [HttpGet("original")]
        public async Task<ActionResult<List<ProductReadByOriginalPrice>>> GetAlProductsByComparisionOriginalPrice()
        {
            try
            {
                var products = await _repository.GetAlProductsByComparisionOriginalPrice();
                return Ok(products);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "حدث خطأ أثناء جلب المنتجات.");
                return StatusCode(500, "حدث خطأ أثناء جلب المنتجات.");
            }
        }
        [HttpGet("productId/{id}")]
        public async Task<ActionResult<ProductReadByProductIdDTO>> GetProductByProductIdForAdmin(int id)
        {
            try
            {
                var product = await _repository.GetProductByProductIdAsync(id);
                if (product == null)
                {
                    return NotFound(new { Message = "لم يتم العثور على المنتج." });
                }
                return Ok(new { Message = "تم العثور على المنتج.", Product = product });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "حدث خطأ أثناء جلب المنتج باستخدام المعرف: {Id}.", id);
                return StatusCode(500, new { Message = "حدث خطأ أثناء استرداد المنتج." });
            }
        }

        [HttpDelete("product/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _repository.DeleteProductAsync(id);
                if (result)
                {
                    return StatusCode(204, new { Message = "تم عملية الحذف بنجاح" } ); // 204 No Content
                }

                return StatusCode(404, new { Message = "المنتج الذي تبحث عنه غير موجود" }); // 404 Not Found
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[حدث خطأ أثناء حذف المنتج باستخدام المعرف]: {Id}", id);
                return StatusCode(500, "Internal server error"); // 500 Internal Server Error
            }
        }
        [HttpGet("categoryId/{id}")]
        public async Task<ActionResult<List<ProductReadByCategoryIdDTO>>> GetProductByCategoryId(int id)
        {
            try
            {
                var products = await _repository.GetProductByCategoryIdAsync(id);
                return Ok(products);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "حدث خطأ أثناء جلب المنتجات.");
                return StatusCode(500, new { Message = "حدث خطأ أثناء جلب المنتجات." } );
            }
        }
            [HttpGet("search")]
            public async Task<IActionResult> SearchProducts([FromQuery] string query)
            {
                if (string.IsNullOrEmpty(query))
                {
                    return BadRequest(new { Message = "كلمة البحث لا يجب أن تكون فارغة" });
                }

                var products = await _repository.SearchProductsAsync(query);

                if (products == null || products.Count == 0)
                {
                    return NotFound(new { Message = "لا يوجد منتجات لكلمة البحث هذه" } );
                }

                return Ok(products);
            }
        }
    }
