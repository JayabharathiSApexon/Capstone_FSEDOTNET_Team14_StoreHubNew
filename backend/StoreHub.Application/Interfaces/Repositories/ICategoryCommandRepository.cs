using StoreHub.Domain.Entities;

namespace StoreHub.Application.Interfaces.Repositories
{
    public interface ICategoryCommandRepository
    {
        Task<Category> CreateCategoryAsync(Category category);

        Task<Category> UpdateCategoryAsync(Category category);

        Task<Category> DeleteCategoryAsync(Category category);
    }
}