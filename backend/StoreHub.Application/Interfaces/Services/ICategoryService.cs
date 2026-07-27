using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StoreHub.Application.Models.Category;

namespace StoreHub.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseModel>> GetAllCategoriesAsync();

        Task<CategoryResponseModel?> GetCategoryByIdAsync(Guid categoryId);

        Task<CategoryResponseModel> CreateCategoryAsync(CategoryRequestModel request);

        Task<CategoryResponseModel> UpdateCategoryAsync(CategoryRequestModel request);

        Task<CategoryResponseModel> DeleteCategoryAsync(Guid categoryId);
    }
}