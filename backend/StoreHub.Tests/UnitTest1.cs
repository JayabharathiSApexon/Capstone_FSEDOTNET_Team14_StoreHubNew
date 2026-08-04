using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StoreHub.API.Controllers;
using StoreHub.API.Models.Auth;
using StoreHub.Domain.Entities;
using StoreHub.Infrastructure.Data;

namespace StoreHub.Tests;

public class AuthControllerTests
{
    [Fact]
    public async Task Register_ShouldCreateUserWithHashedPassword_AndReturnToken()
    {
        using var context = BuildInMemoryDbContext();
        var controller = BuildController(context);

        var request = new RegisterRequest
        {
            FullName = "Test Customer",
            Email = "customer@storehub.com",
            Password = "Password@123",
            PhoneNumber = "9999999999"
        };

        var result = await controller.Register(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<AuthResponse>(okResult.Value);

        Assert.False(string.IsNullOrWhiteSpace(response.Token));
        Assert.Equal("customer@storehub.com", response.Email);
        Assert.False(response.IsAdmin);

        var user = await context.Users.FirstOrDefaultAsync(user => user.Email == request.Email);
        Assert.NotNull(user);
        Assert.NotEqual(request.Password, user!.PasswordHash);
        Assert.True(BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash));
    }

    [Fact]
    public async Task Register_ShouldReturnConflict_WhenEmailAlreadyExists()
    {
        using var context = BuildInMemoryDbContext();

        await context.Users.AddAsync(new User
        {
            Id = Guid.NewGuid(),
            FullName = "Existing User",
            Email = "existing@storehub.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password@123"),
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        var controller = BuildController(context);

        var result = await controller.Register(new RegisterRequest
        {
            FullName = "Duplicate",
            Email = "existing@storehub.com",
            Password = "Password@456"
        });

        Assert.IsType<ConflictObjectResult>(result);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenPasswordIsInvalid()
    {
        using var context = BuildInMemoryDbContext();

        await context.Users.AddAsync(new User
        {
            Id = Guid.NewGuid(),
            FullName = "Login User",
            Email = "login@storehub.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword@123"),
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        var controller = BuildController(context);

        var result = await controller.Login(new LoginRequest
        {
            Email = "login@storehub.com",
            Password = "WrongPassword@123"
        });

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Login_ShouldReturnToken_WithAdminRoleClaim_ForAdminUser()
    {
        using var context = BuildInMemoryDbContext();

        await context.Users.AddAsync(new User
        {
            Id = Guid.NewGuid(),
            FullName = "Admin User",
            Email = "admin@storehub.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("AdminPassword@123"),
            IsAdmin = true,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        var controller = BuildController(context);

        var result = await controller.Login(new LoginRequest
        {
            Email = "admin@storehub.com",
            Password = "AdminPassword@123"
        });

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<AuthResponse>(okResult.Value);

        var token = new JwtSecurityTokenHandler().ReadJwtToken(response.Token);
        var roleClaim = token.Claims.FirstOrDefault(claim =>
            claim.Type == "role" ||
            claim.Type == ClaimTypes.Role)?.Value;

        Assert.Equal("Admin", roleClaim);
        Assert.True(response.IsAdmin);
    }

    [Fact]
    public async Task Login_ShouldAllowUsername_AndMigratePlainTextPasswordToBcrypt()
    {
        using var context = BuildInMemoryDbContext();

        var userId = Guid.NewGuid();
        const string plainPassword = "LegacyPassword@123";

        await context.Users.AddAsync(new User
        {
            Id = userId,
            FullName = "Admin@apexon.com",
            Email = "admin-legacy@storehub.com",
            PasswordHash = plainPassword,
            IsAdmin = true,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        var controller = BuildController(context);

        var result = await controller.Login(new LoginRequest
        {
            Email = "Admin@apexon.com",
            Password = plainPassword
        });

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<AuthResponse>(okResult.Value);

        Assert.False(string.IsNullOrWhiteSpace(response.Token));

        var updatedUser = await context.Users.FirstAsync(user => user.Id == userId);
        Assert.NotEqual(plainPassword, updatedUser.PasswordHash);
        Assert.True(BCrypt.Net.BCrypt.Verify(plainPassword, updatedUser.PasswordHash));
    }

    private static AppDbContext BuildInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static AuthController BuildController(AppDbContext context)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "TestJwtSigningKey_StoreHub_Auth_Controller_2026",
                ["Jwt:Issuer"] = "StoreHub.API",
                ["Jwt:Audience"] = "StoreHub.Client",
                ["Jwt:ExpiryMinutes"] = "60"
            })
            .Build();

        return new AuthController(context, configuration);
    }
}