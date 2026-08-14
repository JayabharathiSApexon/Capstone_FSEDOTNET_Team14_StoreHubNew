using StoreHub.Application.Models.Profile;

namespace StoreHub.Application.Interfaces.Services
{
    public interface IProfileService
    {
        Task<ProfileResponse?> GetProfileAsync(Guid userId);

        Task<UpdateProfileResult> UpdateProfileAsync(Guid userId, UpdateProfileRequest request);
    }
}