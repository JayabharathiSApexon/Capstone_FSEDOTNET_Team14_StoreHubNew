using StoreHub.Domain.Entities;

namespace StoreHub.Application.Interfaces.Repositories
{
    public interface ICategoryQueryRepository
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();

        Task<Category?> GetCategoryByIdAsync(Guid categoryId);
    }
}