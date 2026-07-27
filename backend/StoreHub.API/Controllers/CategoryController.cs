using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StoreHub.Application.Interfaces.Services;
using StoreHub.Application.Models.Category;

namespace StoreHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{categoryId:guid}")]
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
        public async Task<IActionResult> CreateCategory(CategoryRequestModel request)
        {
            var category = await _categoryService.CreateCategoryAsync(request);

            return CreatedAtAction(
                nameof(GetCategoryById),
                new { categoryId = category.Id },
                category);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCategory(CategoryRequestModel request)
        {
            var category = await _categoryService.UpdateCategoryAsync(request);

            return Ok(category);
        }

        [HttpDelete("{categoryId:guid}")]
        public async Task<IActionResult> DeleteCategory(Guid categoryId)
        {
            var category = await _categoryService.DeleteCategoryAsync(categoryId);

            return Ok(category);
        }
    }
}