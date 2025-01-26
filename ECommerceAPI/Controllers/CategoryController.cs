using ECommerceCore.DTOs.Category;
using ECommerceInfrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllOrigins")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _repository;

        public CategoryController(ICategoryRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategoriesForUser()
        {
            try
            {
                Log.Information("جارِ جلب جميع الفئات للمستخدم.");
                var categories = await _repository.GetAllCategoriesForUserAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "حدث خطأ أثناء جلب الفئات للمستخدم.");
                return StatusCode(500, "حدث خطأ أثناء استرداد الفئات.");
            }
        }

        [HttpGet("Admin")]
        public async Task<IActionResult> GetAllCategoriesForAdmin()
        {
            try
            {
                Log.Information("جارِ جلب جميع الفئات للمشرف.");
                var categories = await _repository.GetAllCategoriesForAdminAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "حدث خطأ أثناء جلب الفئات للمشرف.");
                return StatusCode(500, "حدث خطأ أثناء استرداد الفئات.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById([FromRoute] int id)
        {
            try
            {
                Log.Information("جارِ جلب الفئة باستخدام المعرف: {Id}.", id);
                var category = await _repository.GetCategoryByIdAsync(id);

                if (category == null)
                    return NotFound(new { Message = "لم يتم العثور على الفئة." });

                return Ok(new { Message = "تم العثور على الفئة.", Category = category });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "حدث خطأ أثناء جلب الفئة باستخدام المعرف: {Id}.", id);
                return StatusCode(500, new { Message = "حدث خطأ أثناء استرداد الفئة." });
            }
        }

        [HttpGet("name/{id}")]
        public async Task<IActionResult> GetCategoryReadForIdName([FromRoute] int id)
        {
            try
            {
                var category = await _repository.GetCategoryReadForIdName(id);
                if (category == null)
                    return NotFound(new { Message = "لم يتم العثور على الفئة." });
                return Ok(new { Message = "تم العثور على الفئة.", Category = category });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "حدث خطأ أثناء جلب الفئة باستخدام المعرف: {Id}.", id);
                return StatusCode(500, new { Message = "حدث خطأ أثناء استرداد الفئة." });
            }
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> CreateCategory([FromForm] CategoryCreateDTO categoryDto)
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

            var category = await _repository.CreateCategoryAsync(categoryDto);

            if (category != null)
            {
                return BadRequest(new { errors = category });
            }

            return Ok(new { Message = "تم إنشاء الفئة بنجاح" });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory([FromRoute] int id, [FromForm] CategoryUpdateDTO categoryDto)
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

            var category = await _repository.UpdateCategoryAsync(id, categoryDto);

            if (category != null)
            {
                return BadRequest(new { errors = category });
            }

            return Ok(new { Message = "تم تحديث الفئة بنجاح" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory([FromRoute] int id)
        {
            var category = await _repository.DeleteCategoryAsync(id);

            if (category != null)
            {
                return BadRequest(new { errors = category });
            }

            return Ok(new { Message = "تم حذف الفئة بنجاح" });
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchCategoriesByName([FromQuery] string name)
        {
            try
            {
                var categories = await _repository.SearchCategoriesByNameAsync(name);
                if (!categories.Any()) return NotFound("لم يتم العثور على أي فئة مطابقة.");
                return Ok(new { Message = "تم العثور على الفئات.", Categories = categories });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "حدث خطأ أثناء البحث عن الفئات.");
                return StatusCode(500, "حدث خطأ أثناء البحث عن الفئات.");
            }
        }
    }
}