using StoreHub.API.Models.User;

namespace StoreHub.API.Services.Interfaces
{
    public interface IUserQueryService
    {
        Task<IReadOnlyList<UserListItemResponse>> GetAllUsersAsync();
    }
}