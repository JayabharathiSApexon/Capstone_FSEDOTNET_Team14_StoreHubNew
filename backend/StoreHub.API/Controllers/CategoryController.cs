using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreHub.API.Models.Category;
using StoreHub.API.Services.Interfaces;
using StoreHub.Application.Interfaces.Services;

namespace StoreHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ICategoryRequestMapper _categoryRequestMapper;

        public CategoryController(
            ICategoryService categoryService,
            ICategoryRequestMapper categoryRequestMapper)
        {
            _categoryService = categoryService;
            _categoryRequestMapper = categoryRequestMapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{categoryId:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoryById(Guid categoryId)
        {
            var category = await _categoryService.GetCategoryByIdAsync(categoryId);

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory(CategoryRequest request)
        {
            var categoryRequest = _categoryRequestMapper.ToApplicationModel(request);
            var category = await _categoryService.CreateCategoryAsync(categoryRequest);

            return CreatedAtAction(
                nameof(GetCategoryById),
                new { categoryId = category.Id },
                category);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(CategoryRequest request)
        {
            var categoryRequest = _categoryRequestMapper.ToApplicationModel(request);
            var category = await _categoryService.UpdateCategoryAsync(categoryRequest);

            return Ok(category);
        }

        [HttpDelete("{categoryId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(Guid categoryId)
        {
            var category = await _categoryService.DeleteCategoryAsync(categoryId);

            return Ok(category);
        }
    }
}