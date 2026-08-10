using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StoreHub.API.Models.Auth;
using StoreHub.Domain.Entities;
using StoreHub.Infrastructure.Data;

namespace StoreHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var normalizedEmail = request.Email.Trim().ToLowerInvariant();

            var emailExists = await _context.Users
                .AnyAsync(user => user.Email.ToLower() == normalizedEmail && user.IsActive);

            if (emailExists)
            {
                return Conflict(new { message = "Email is already registered." });
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = request.FullName.Trim(),
                Email = normalizedEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                PhoneNumber = request.PhoneNumber?.Trim() ?? string.Empty,
                IsAdmin = false,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var response = CreateAuthResponse(user);

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var identifier = request.Email.Trim();
            var normalizedIdentifier = identifier.ToLowerInvariant();

            var user = await _context.Users
                .FirstOrDefaultAsync(existingUser =>
                    (existingUser.Email.ToLower() == normalizedIdentifier ||
                     existingUser.FullName.ToLower() == normalizedIdentifier) &&
                    existingUser.IsActive);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            var passwordValid = false;

            try
            {
                passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            }
            catch
            {
                passwordValid = false;
            }

            if (!passwordValid && user.PasswordHash == request.Password)
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                user.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                passwordValid = true;
            }

            if (!passwordValid)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            var response = CreateAuthResponse(user);

            return Ok(response);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (request.NewPassword != request.ConfirmPassword)
            {
                return BadRequest(new { message = "Password and confirm password do not match." });
            }

            var normalizedEmail = request.Email.Trim().ToLowerInvariant();

            var user = await _context.Users
                .FirstOrDefaultAsync(existingUser =>
                    existingUser.Email.ToLower() == normalizedEmail &&
                    existingUser.IsActive);

            if (user == null)
            {
                return NotFound(new { message = "Account not found." });
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Password reset successful. Please login with your new password." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (request.NewPassword != request.ConfirmPassword)
            {
                return BadRequest(new { message = "Password and confirm password do not match." });
            }

            var normalizedEmail = request.Email.Trim().ToLowerInvariant();

            var user = await _context.Users
                .FirstOrDefaultAsync(existingUser =>
                    existingUser.Email.ToLower() == normalizedEmail &&
                    existingUser.IsActive);

            if (user == null)
            {
                return BadRequest(new { message = "Unable to reset password for this account." });
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Password reset successful. Please login with your new password." });
        }

        private AuthResponse CreateAuthResponse(User user)
        {
            var expiryMinutes = _configuration.GetValue<int?>("Jwt:ExpiryMinutes") ?? 60;
            var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var token = GenerateJwtToken(user, expiresAt);

            return new AuthResponse
            {
                Token = token,
                ExpiresAtUtc = expiresAt,
                UserId = user.Id.ToString(),
                FullName = user.FullName,
                Email = user.Email,
                IsAdmin = user.IsAdmin
            };
        }

        private string GenerateJwtToken(User user, DateTime expiresAt)
        {
            var jwtKey = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT key is not configured.");

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.UniqueName, user.FullName),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, user.FullName),
                new(ClaimTypes.Role, user.IsAdmin ? "Admin" : "Customer")
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expiresAt,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
