using StoreHub.Application.Interfaces.Repositories;
using StoreHub.API.Services.Interfaces;
using StoreHub.Domain.Entities;

namespace StoreHub.API.Services
{
    public class AdminSeedService : IAdminSeedService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher _passwordHasher;

        public AdminSeedService(
            IUserRepository userRepository,
            IConfiguration configuration,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
        }

        public async Task SeedAdminUserAsync()
        {
            var adminEmail = _configuration["AdminSeed:Email"]?.Trim().ToLowerInvariant();
            var adminPassword = _configuration["AdminSeed:Password"]?.Trim();
            var adminFullName = _configuration["AdminSeed:FullName"]?.Trim();

            if (string.IsNullOrWhiteSpace(adminEmail) ||
                string.IsNullOrWhiteSpace(adminPassword) ||
                string.IsNullOrWhiteSpace(adminFullName))
            {
                return;
            }

            var existingAdmin = await _userRepository.GetByEmailAsync(adminEmail);

            if (existingAdmin == null)
            {
                await _userRepository.AddAsync(new User
                {
                    Id = Guid.NewGuid(),
                    FullName = adminFullName,
                    Email = adminEmail,
                    PasswordHash = _passwordHasher.Hash(adminPassword),
                    IsAdmin = true,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                });

                await _userRepository.SaveChangesAsync();
                return;
            }

            var updated = false;

            if (!existingAdmin.IsAdmin)
            {
                existingAdmin.IsAdmin = true;
                updated = true;
            }

            if (!existingAdmin.IsActive)
            {
                existingAdmin.IsActive = true;
                updated = true;
            }

            if (!existingAdmin.PasswordHash.StartsWith("$2") && existingAdmin.PasswordHash == adminPassword)
            {
                existingAdmin.PasswordHash = _passwordHasher.Hash(adminPassword);
                updated = true;
            }

            if (updated)
            {
                existingAdmin.UpdatedDate = DateTime.UtcNow;
                await _userRepository.SaveChangesAsync();
            }
        }
    }
}