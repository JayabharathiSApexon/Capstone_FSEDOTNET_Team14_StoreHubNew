using StoreHub.Application.Interfaces.Repositories;
using StoreHub.API.Models.User;
using StoreHub.API.Services.Interfaces;

namespace StoreHub.API.Services
{
    public class UserQueryService : IUserQueryService
    {
        private readonly IUserRepository _userRepository;

        public UserQueryService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IReadOnlyList<UserListItemResponse>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersOrderedByCreatedDateDescAsync();

            return users
                .Select(user => new UserListItemResponse
                {
                    Id = user.Id.ToString(),
                    FullName = user.FullName,
                    Email = user.Email,
                    IsAdmin = user.IsAdmin,
                    Role = user.IsAdmin ? "Admin" : "Guest/User",
                    IsActive = user.IsActive,
                    CreatedDate = user.CreatedDate
                })
                .ToList();
        }
    }
}