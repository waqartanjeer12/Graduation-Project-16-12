using ECommerceCore.DTOs.Color;
using ECommerceCore.DTOs.Product;
using ECommerceCore.Interfaces;
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
                Log.Error(ex, "حدث خطأ أثناء اضافة.");
                return StatusCode(500, "حدث خطأ أثناء اضافة.");
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
                Log.Error(ex, "حدث خطأ انشاء انشاء المنتج");
                return StatusCode(500, "حدث خطأ أثناء انشاء المنتج");
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
    }
}