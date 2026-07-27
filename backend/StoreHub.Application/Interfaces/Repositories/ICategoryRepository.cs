using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StoreHub.Domain.Entities;

namespace StoreHub.Application.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();

        Task<Category?> GetCategoryByIdAsync(Guid categoryId);

        Task<Category> CreateCategoryAsync(Category category);

        Task<Category> UpdateCategoryAsync(Category category);

        Task<Category> DeleteCategoryAsync(Category category);
    }
}