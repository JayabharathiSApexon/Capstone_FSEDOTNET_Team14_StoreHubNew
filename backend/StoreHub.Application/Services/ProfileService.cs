using StoreHub.Application.Interfaces.Repositories;
using StoreHub.Application.Interfaces.Services;
using StoreHub.Application.Models.Profile;

namespace StoreHub.Application.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUserRepository _userRepository;

        public ProfileService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ProfileResponse?> GetProfileAsync(Guid userId)
        {
            var user = await _userRepository.GetActiveUserByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            return new ProfileResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsAdmin = user.IsAdmin,
                CreatedDate = user.CreatedDate
            };
        }

        public async Task<UpdateProfileResult> UpdateProfileAsync(Guid userId, UpdateProfileRequest request)
        {
            var user = await _userRepository.GetActiveUserByIdAsync(userId);

            if (user == null)
            {
                return UpdateProfileResult.UserNotFound;
            }

            var normalizedEmail = request.Email.Trim().ToLowerInvariant();

            var existingUser = await _userRepository.GetByEmailAsync(normalizedEmail);

            if (existingUser != null && existingUser.Id != userId)
            {
                return UpdateProfileResult.EmailAlreadyExists;
            }

            user.FullName = request.FullName.Trim();
            user.Email = normalizedEmail;
            user.PhoneNumber = request.PhoneNumber?.Trim() ?? string.Empty;
            user.UpdatedDate = DateTime.UtcNow;

            await _userRepository.SaveChangesAsync();

            return UpdateProfileResult.Success;
        }
    }
}