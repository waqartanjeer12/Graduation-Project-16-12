using ECommerceCore.DTOs.Category;
using ECommerceInfrastructure.Repositories;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Serilog;

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




        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromForm] CategoryCreateDTO categoryDto)
        {
            try
            {
                var category = await _repository.CreateCategoryAsync(categoryDto);
                return Ok(new { Message = "تم إنشاء الفئة بنجاح", CategoryId = category.Id });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "حدث خطأ أثناء إنشاء الفئة.");
                return StatusCode(500, "حدث خطأ أثناء إنشاء الفئة.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory([FromRoute] int id, [FromForm] CategoryUpdateDTO categoryDto)
        {
            try
            {
                // Call the repository to update the category and get the updated category object
                var category = await _repository.UpdateCategoryAsync(id, categoryDto);

                if (category == null)
                {
                    return NotFound("لم يتم العثور على الفئة."); // Category not found
                }

                return Ok(new { Message = "تم تحديث الفئة بنجاح", Category = category }); // Return the updated category
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "حدث خطأ أثناء تحديث الفئة."); // Internal server error
            }
        }





        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] int id)
        {
            try
            {
                var success = await _repository.DeleteCategoryAsync(id);
                if (!success) return NotFound("لم يتم العثور على الفئة.");
                return Ok(new { Message = "تم حذف الفئة بنجاح" });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "حدث خطأ أثناء حذف الفئة.");
                return StatusCode(500, "حدث خطأ أثناء حذف الفئة.");
            }
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
