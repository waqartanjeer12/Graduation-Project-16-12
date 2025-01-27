using ECommerceCore.DTOs.Color;
using ECommerceCore.DTOs.Product;
using ECommerceCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Collections.Generic;
using System.Linq;
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateColorAsync([FromForm] ColorCreateDTO colorDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(ms => ms.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return BadRequest(new { errors });
            }

            var errorsFromService = await _repository.CreateColorAsync(colorDto);

            if (errorsFromService != null)
            {
                return BadRequest(new { errors = errorsFromService });
            }

            return Ok(new { Message = "تم إنشاء اللون بنجاح" });
        }

        [HttpDelete("Color/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteColorAsync(int id)
        {
            var errorsFromService = await _repository.DeleteColorAsync(id);

            if (errorsFromService != null)
            {
                return BadRequest(new { errors = errorsFromService });
            }

            return StatusCode(204, new { Message = "تم حذف اللون بنجاح." }); // 204 No Content
        }

        [HttpPut("Color/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateColorAsync([FromRoute] int id, [FromForm] ColorUpdateDTO colorDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(ms => ms.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return BadRequest(new { errors });
            }

            var errorsFromService = await _repository.UpdateColorAsync(id, colorDto);

            if (errorsFromService != null)
            {
                return BadRequest(new { errors = errorsFromService });
            }

            return Ok(new { Message = "تم تحديث اللون بنجاح" });
        }

        [HttpGet("Color/{id}")]
        public async Task<IActionResult> GetColorByIdAsync(int id)
        {
            try
            {
                Log.Information("جارِ جلب اللون باستخدام المعرف: {Id}", id);
                var color = await _repository.GetColorByIdAsync(id);

                if (color == null)
                {
                    return NotFound(new { Message = "لم يتم العثور على اللون." });
                }

                return Ok(new { Message = "تم العثور على اللون.", Color = color });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[حدث خطأ أثناء جلب اللون باستخدام المعرف]: {Id}", id);
                return StatusCode(500, new { Message = "حدث خطأ أثناء استرداد اللون." });
            }
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProductAsync([FromForm] ProductCreateDTO productCreateDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(ms => ms.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return BadRequest(new { errors });
            }

            var errorsFromService = await _repository.CreateProductAsync(productCreateDTO);

            if (errorsFromService != null)
            {
                return BadRequest(new { errors = errorsFromService });
            }

            return Ok(new { Message = "تم إنشاء المنتج بنجاح" });
        }

        [HttpPut("update/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProductAsync(int id, [FromForm] ProductUpdateDTO productUpdateDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(ms => ms.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return BadRequest(new { errors });
            }

            var errorsFromService = await _repository.UpdateProductAsync(id, productUpdateDTO);

            if (errorsFromService != null)
            {
                return BadRequest(new { errors = errorsFromService });
            }

            return Ok(new { Message = "تم تحديث المنتج بنجاح" });
        }

        [HttpDelete("product/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var errorsFromService = await _repository.DeleteProductAsync(id);

            if (errorsFromService != null)
            {
                return BadRequest(new { errors = errorsFromService });
            }

            return StatusCode(204, new { Message = "تم حذف المنتج بنجاح" }); // 204 No Content
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
                return StatusCode(500, new { Message = "حدث خطأ أثناء جلب المنتجات." });
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
                return NotFound(new { Message = "لا يوجد منتجات لكلمة البحث هذه" });
            }

            return Ok(products);
        }
    }
}