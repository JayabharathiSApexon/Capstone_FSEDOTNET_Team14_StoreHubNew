using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using StoreHub.API.Models.Auth;
using StoreHub.API.Services.Interfaces;
using StoreHub.Domain.Entities;

namespace StoreHub.API.Services
{
    public class JwtAuthTokenFactory : IAuthTokenFactory
    {
        private readonly IConfiguration _configuration;

        public JwtAuthTokenFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public AuthResponse Create(User user)
        {
            var expiryMinutes = _configuration.GetValue<int?>("Jwt:ExpiryMinutes") ?? 60;
            var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);
            var token = GenerateToken(user, expiresAt);

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

        private string GenerateToken(User user, DateTime expiresAt)
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