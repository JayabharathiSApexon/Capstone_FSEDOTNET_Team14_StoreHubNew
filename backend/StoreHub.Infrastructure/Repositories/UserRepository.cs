using Microsoft.EntityFrameworkCore;
using StoreHub.Application.Interfaces.Repositories;
using StoreHub.Domain.Entities;
using StoreHub.Infrastructure.Data;

namespace StoreHub.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsActiveByEmailAsync(string normalizedEmail)
        {
            return await _context.Users
                .AnyAsync(user => user.Email.ToLower() == normalizedEmail && user.IsActive);
        }

        public async Task<User?> GetActiveByEmailOrFullNameAsync(string normalizedIdentifier)
        {
            return await _context.Users
                .FirstOrDefaultAsync(user =>
                    (user.Email.ToLower() == normalizedIdentifier ||
                     user.FullName.ToLower() == normalizedIdentifier) &&
                    user.IsActive);
        }

        public async Task<User?> GetActiveByEmailAsync(string normalizedEmail)
        {
            return await _context.Users
                .FirstOrDefaultAsync(user => user.Email.ToLower() == normalizedEmail && user.IsActive);
        }

        public async Task<User?> GetByEmailAsync(string normalizedEmail)
        {
            return await _context.Users
                .FirstOrDefaultAsync(user => user.Email.ToLower() == normalizedEmail);
        }

        public async Task<IReadOnlyList<User>> GetAllUsersOrderedByCreatedDateDescAsync()
        {
            return await _context.Users
                .OrderByDescending(user => user.CreatedDate)
                .ToListAsync();
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}