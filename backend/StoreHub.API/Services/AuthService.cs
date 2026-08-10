using Microsoft.AspNetCore.Http;
using StoreHub.Application.Interfaces.Repositories;
using StoreHub.API.Models.Auth;
using StoreHub.API.Services.Interfaces;
using StoreHub.API.Services.Models;
using StoreHub.Domain.Entities;

namespace StoreHub.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAuthTokenFactory _authTokenFactory;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IAuthTokenFactory authTokenFactory)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _authTokenFactory = authTokenFactory;
        }

        public async Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();

            var emailExists = await _userRepository.ExistsActiveByEmailAsync(normalizedEmail);

            if (emailExists)
            {
                return ServiceResult<AuthResponse>.Failure(
                    StatusCodes.Status409Conflict,
                    "Email is already registered.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = request.FullName.Trim(),
                Email = normalizedEmail,
                PasswordHash = _passwordHasher.Hash(request.Password),
                PhoneNumber = request.PhoneNumber?.Trim() ?? string.Empty,
                IsAdmin = false,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return ServiceResult<AuthResponse>.Success(_authTokenFactory.Create(user));
        }

        public async Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request)
        {
            var identifier = request.Email.Trim();
            var normalizedIdentifier = identifier.ToLowerInvariant();

            var user = await _userRepository.GetActiveByEmailOrFullNameAsync(normalizedIdentifier);

            if (user == null)
            {
                return ServiceResult<AuthResponse>.Failure(
                    StatusCodes.Status401Unauthorized,
                    "Invalid email or password.");
            }

            var passwordValid = false;

            try
            {
                passwordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);
            }
            catch
            {
                passwordValid = false;
            }

            if (!passwordValid && user.PasswordHash == request.Password)
            {
                user.PasswordHash = _passwordHasher.Hash(request.Password);
                user.UpdatedDate = DateTime.UtcNow;
                await _userRepository.SaveChangesAsync();
                passwordValid = true;
            }

            if (!passwordValid)
            {
                return ServiceResult<AuthResponse>.Failure(
                    StatusCodes.Status401Unauthorized,
                    "Invalid email or password.");
            }

            return ServiceResult<AuthResponse>.Success(_authTokenFactory.Create(user));
        }

        public async Task<ServiceResult<string>> ForgotPasswordAsync(ResetPasswordRequest request)
        {
            return await UpdatePasswordAsync(
                request,
                StatusCodes.Status404NotFound,
                "Account not found.");
        }

        public async Task<ServiceResult<string>> ResetPasswordAsync(ResetPasswordRequest request)
        {
            return await UpdatePasswordAsync(
                request,
                StatusCodes.Status400BadRequest,
                "Unable to reset password for this account.");
        }

        private async Task<ServiceResult<string>> UpdatePasswordAsync(
            ResetPasswordRequest request,
            int missingUserStatusCode,
            string missingUserMessage)
        {
            if (request.NewPassword != request.ConfirmPassword)
            {
                return ServiceResult<string>.Failure(
                    StatusCodes.Status400BadRequest,
                    "Password and confirm password do not match.");
            }

            var normalizedEmail = request.Email.Trim().ToLowerInvariant();

            var user = await _userRepository.GetActiveByEmailAsync(normalizedEmail);

            if (user == null)
            {
                return ServiceResult<string>.Failure(missingUserStatusCode, missingUserMessage);
            }

            user.PasswordHash = _passwordHasher.Hash(request.NewPassword);
            user.UpdatedDate = DateTime.UtcNow;

            await _userRepository.SaveChangesAsync();

            return ServiceResult<string>.Success(
                "Password reset successful. Please login with your new password.");
        }
    }
}