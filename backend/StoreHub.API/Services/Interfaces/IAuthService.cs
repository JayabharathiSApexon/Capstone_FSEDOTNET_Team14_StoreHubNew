using StoreHub.API.Models.Auth;
using StoreHub.API.Services.Models;

namespace StoreHub.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request);

        Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request);

        Task<ServiceResult<string>> ForgotPasswordAsync(ResetPasswordRequest request);

        Task<ServiceResult<string>> ResetPasswordAsync(ResetPasswordRequest request);
    }
}