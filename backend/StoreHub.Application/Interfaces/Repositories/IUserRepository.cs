using StoreHub.Domain.Entities;

namespace StoreHub.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<bool> ExistsActiveByEmailAsync(string normalizedEmail);

        Task<User?> GetActiveByEmailOrFullNameAsync(string normalizedIdentifier);

        Task<User?> GetActiveByEmailAsync(string normalizedEmail);

        Task<User?> GetByEmailAsync(string normalizedEmail);

        Task<IReadOnlyList<User>> GetAllUsersOrderedByCreatedDateDescAsync();

        Task<User?> GetActiveUserByIdAsync(Guid userId);

        Task AddAsync(User user);

        Task SaveChangesAsync();
    }
}