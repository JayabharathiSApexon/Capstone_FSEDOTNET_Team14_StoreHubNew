using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using StoreHub.API.Mappings;
using StoreHub.API.Models.Auth;
using StoreHub.API.Models.Category;
using StoreHub.API.Models.Product;
using StoreHub.API.Models.User;
using StoreHub.API.Services;
using StoreHub.API.Services.Interfaces;
using StoreHub.Application.Interfaces.Repositories;
using StoreHub.Application.Models.Category;
using StoreHub.Application.Models.Product;
using StoreHub.Domain.Entities;

namespace StoreHub.Tests.Services;

public class ApiServiceTests
{
    [Fact]
    public void Given_PlainTextPassword_When_HashAndVerify_Then_ReturnsTrue()
    {
        var hasher = new BCryptPasswordHasher();

        var hash = hasher.Hash("Password@123");

        Assert.NotEqual("Password@123", hash);
        Assert.True(hasher.Verify("Password@123", hash));
    }

    [Fact]
    public void Given_WrongPassword_When_Verify_Then_ReturnsFalse()
    {
        var hasher = new BCryptPasswordHasher();
        var hash = hasher.Hash("Password@123");

        var result = hasher.Verify("WrongPassword@123", hash);

        Assert.False(result);
    }

    [Fact]
    public void Given_User_When_Create_Then_ReturnsTokenWithExpectedClaims()
    {
        var configuration = BuildConfiguration();
        var factory = new JwtAuthTokenFactory(configuration);
        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = "Jane Doe",
            Email = "jane@test.com",
            IsAdmin = true
        };

        var result = factory.Create(user);

        Assert.Equal(user.Id.ToString(), result.UserId);
        Assert.Equal(user.FullName, result.FullName);
        Assert.Equal(user.Email, result.Email);
        Assert.True(result.IsAdmin);
        Assert.False(string.IsNullOrWhiteSpace(result.Token));

        var token = new JwtSecurityTokenHandler().ReadJwtToken(result.Token);
        Assert.Equal(user.Email, token.Claims.First(x => x.Type == JwtRegisteredClaimNames.Email).Value);
        Assert.Equal("Admin", token.Claims.First(x => x.Type == ClaimTypes.Role).Value);
        Assert.True(result.ExpiresAtUtc > DateTime.UtcNow);
    }

    [Fact]
    public void Given_MissingExpiryMinutes_When_Create_Then_UsesDefaultExpiry()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "ThisIsASecretKeyForJwtTests_1234567890",
                ["Jwt:Issuer"] = "StoreHub.API",
                ["Jwt:Audience"] = "StoreHub.Client"
            })
            .Build();

        var factory = new JwtAuthTokenFactory(configuration);
        var result = factory.Create(new User
        {
            Id = Guid.NewGuid(),
            FullName = "Guest User",
            Email = "guest@test.com",
            IsAdmin = false
        });

        Assert.InRange((result.ExpiresAtUtc - DateTime.UtcNow).TotalMinutes, 59, 61);
    }

    [Fact]
    public void Given_MissingJwtKey_When_Create_Then_ThrowsConfigurationException()
    {
        var configuration = new ConfigurationBuilder().Build();
        var factory = new JwtAuthTokenFactory(configuration);

        var exception = Assert.Throws<InvalidOperationException>(() => factory.Create(new User
        {
            Id = Guid.NewGuid(), FullName = "User", Email = "user@test.com"
        }));

        Assert.Equal("JWT key is not configured.", exception.Message);
    }

    [Fact]
    public async Task Given_Images_When_SaveNewImagesAsync_Then_ReturnsOrderedPrimaryImages()
    {
        var environment = new TestWebHostEnvironment(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")));
        var service = new ProductImageStorageService(environment);
        var images = new List<IFormFile>
        {
            CreateFormFile("first.png", "first-content"),
            CreateFormFile("second.png", "second-content")
        };

        var result = await service.SaveNewImagesAsync(images);

        Assert.Equal(2, result.Count);
        Assert.True(result[0].IsPrimary);
        Assert.Equal(1, result[0].DisplayOrder);
        Assert.False(result[1].IsPrimary);
        Assert.Equal(2, result[1].DisplayOrder);
        Assert.Contains("/uploads/products/", result[0].ImageUrl);
        Assert.Contains("/uploads/products/", result[1].ImageUrl);
    }

    [Fact]
    public async Task Given_NoImages_When_SaveNewImagesAsync_Then_ReturnsEmptyList()
    {
        var service = new ProductImageStorageService(new TestWebHostEnvironment(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"))));

        var result = await service.SaveNewImagesAsync(Array.Empty<IFormFile>());

        Assert.Empty(result);
    }

    [Fact]
    public async Task Given_ExistingImage_When_ReplaceImagesAsync_Then_DeletesOldImageAndSavesNewOnes()
    {
        var webRootPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var uploadDirectory = Path.Combine(webRootPath, "uploads", "products");
        Directory.CreateDirectory(uploadDirectory);
        var existingFile = Path.Combine(uploadDirectory, "existing.png");
        await File.WriteAllBytesAsync(existingFile, new byte[] { 1, 2, 3, 4 });

        var service = new ProductImageStorageService(new TestWebHostEnvironment(webRootPath));

        var result = await service.ReplaceImagesAsync(new[] { "/uploads/products/existing.png" },
            new List<IFormFile> { CreateFormFile("replaced.png", "replacement") });

        Assert.Single(result);
        Assert.False(File.Exists(existingFile));
        Assert.StartsWith("/uploads/products/", result[0].ImageUrl);
    }

    [Fact]
    public async Task Given_Users_When_GetAllUsersAsync_Then_MapsToResponseModel()
    {
        var users = new List<User>
        {
            new() { Id = Guid.NewGuid(), FullName = "Admin User", Email = "admin@test.com", IsAdmin = true, IsActive = true, CreatedDate = DateTime.UtcNow.AddDays(-2) },
            new() { Id = Guid.NewGuid(), FullName = "Guest User", Email = "guest@test.com", IsAdmin = false, IsActive = false, CreatedDate = DateTime.UtcNow.AddDays(-1) }
        };
        var repository = new StubUserRepository { AllUsers = users };
        var service = new UserQueryService(repository);

        var result = await service.GetAllUsersAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("Admin", result[0].Role);
        Assert.Equal("Guest/User", result[1].Role);
        Assert.Equal(users[0].Email, result[0].Email);
        Assert.False(result[1].IsActive);
    }

    [Fact]
    public void Given_CategoryRequest_When_ToApplicationModel_Then_MapsProperties()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new ApiMappingProfile()));
        var mapper = new CategoryRequestMapper(configuration.CreateMapper());

        var result = mapper.ToApplicationModel(new CategoryRequest
        {
            Id = Guid.NewGuid(),
            Name = "Electronics",
            Description = "Devices",
            IsActive = true
        });

        Assert.Equal("Electronics", result.Name);
        Assert.Equal("Devices", result.Description);
        Assert.True(result.IsActive);
    }

    [Fact]
    public void Given_CreateProductRequest_When_ToCreateModel_Then_MapsPropertiesAndImages()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new ApiMappingProfile()));
        var mapper = new ProductRequestMapper(configuration.CreateMapper());
        var images = new List<ProductImageRequestModel>
        {
            new() { ImageUrl = "/uploads/products/a.png", IsPrimary = true, DisplayOrder = 1 }
        };

        var result = mapper.ToCreateModel(new CreateProductRequest
        {
            Name = "Laptop",
            CategoryId = Guid.NewGuid(),
            Description = "Gaming",
            Brand = "Dell",
            Price = 900m,
            StockQuantity = 11,
            IsFeatured = true,
            IsActive = true
        }, images);

        Assert.Equal("Laptop", result.Name);
        Assert.Equal("Dell", result.Brand);
        Assert.Equal(900m, result.Price);
        Assert.Single(result.Images);
        Assert.Equal("/uploads/products/a.png", result.Images[0].ImageUrl);
    }

    [Fact]
    public void Given_UpdateProductRequest_When_ToUpdateModel_Then_MapsPropertiesAndImages()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new ApiMappingProfile()));
        var mapper = new ProductRequestMapper(configuration.CreateMapper());
        var images = new List<ProductImageRequestModel>
        {
            new() { ImageUrl = "/uploads/products/b.png", IsPrimary = true, DisplayOrder = 1 },
            new() { ImageUrl = "/uploads/products/c.png", IsPrimary = false, DisplayOrder = 2 }
        };

        var result = mapper.ToUpdateModel(new UpdateProductRequest
        {
            Id = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Name = "Mouse",
            Description = "Wired",
            Brand = "Logitech",
            Price = 49.99m,
            StockQuantity = 15,
            IsFeatured = false,
            IsActive = true
        }, images);

        Assert.Equal("Mouse", result.Name);
        Assert.Equal(2, result.Images.Count);
        Assert.Equal("Logitech", result.Brand);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task Given_NewRequest_When_RegisterAsync_Then_CreatesUserAndReturnsToken()
    {
        var repository = new StubUserRepository();
        var service = new AuthService(repository, new BCryptPasswordHasher(), new StubAuthTokenFactory());

        var result = await service.RegisterAsync(new RegisterRequest
        {
            FullName = "Test User",
            Email = " TEST@EXAMPLE.COM ",
            Password = "Password@123",
            PhoneNumber = " 1234567890 "
        });

        Assert.True(result.Succeeded);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.NotNull(result.Data);
        Assert.Equal("test@example.com", repository.AddedUsers[0].Email);
        Assert.Equal("Test User", repository.AddedUsers[0].FullName);
        Assert.Equal("1234567890", repository.AddedUsers[0].PhoneNumber);
        Assert.NotEqual("Password@123", repository.AddedUsers[0].PasswordHash);
        Assert.True(BCrypt.Net.BCrypt.Verify("Password@123", repository.AddedUsers[0].PasswordHash));
    }

    [Fact]
    public async Task Given_ExistingEmail_When_RegisterAsync_Then_ReturnsConflict()
    {
        var repository = new StubUserRepository { ExistsActiveByEmailResult = true };
        var service = new AuthService(repository, new BCryptPasswordHasher(), new StubAuthTokenFactory());

        var result = await service.RegisterAsync(new RegisterRequest
        {
            FullName = "Duplicate",
            Email = "duplicate@test.com",
            Password = "Password@123"
        });

        Assert.False(result.Succeeded);
        Assert.Equal(StatusCodes.Status409Conflict, result.StatusCode);
        Assert.Empty(repository.AddedUsers);
    }

    [Fact]
    public async Task Given_ExistingUserWithValidPassword_When_LoginAsync_Then_ReturnsSuccess()
    {
        var repository = new StubUserRepository
        {
            ActiveUserByEmailOrName = new User
            {
                Id = Guid.NewGuid(),
                FullName = "Customer",
                Email = "customer@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password@123"),
                IsActive = true,
                IsAdmin = false
            }
        };
        var service = new AuthService(repository, new BCryptPasswordHasher(), new StubAuthTokenFactory());

        var result = await service.LoginAsync(new LoginRequest { Email = "customer@test.com", Password = "Password@123" });

        Assert.True(result.Succeeded);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.Equal("customer@test.com", result.Data!.Email);
    }

    [Fact]
    public async Task Given_UserDoesNotExist_When_LoginAsync_Then_ReturnsUnauthorized()
    {
        var repository = new StubUserRepository();
        var service = new AuthService(repository, new BCryptPasswordHasher(), new StubAuthTokenFactory());

        var result = await service.LoginAsync(new LoginRequest { Email = "missing@test.com", Password = "Password@123" });

        Assert.False(result.Succeeded);
        Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task Given_LegacyPlainTextPassword_When_LoginAsync_Then_MigratesPassword()
    {
        var user = new User
        {
            Id = Guid.NewGuid(), FullName = "Legacy User", Email = "legacy@test.com",
            PasswordHash = "LegacyPassword@123", IsActive = true
        };
        var repository = new StubUserRepository { ActiveUserByEmailOrName = user };
        var service = new AuthService(repository, new BCryptPasswordHasher(), new StubAuthTokenFactory());

        var result = await service.LoginAsync(new LoginRequest { Email = "legacy@test.com", Password = "LegacyPassword@123" });

        Assert.True(result.Succeeded);
        Assert.True(BCrypt.Net.BCrypt.Verify("LegacyPassword@123", user.PasswordHash));
        Assert.Equal(1, repository.SaveChangesCalls);
    }

    [Fact]
    public async Task Given_WrongPassword_When_LoginAsync_Then_ReturnsUnauthorized()
    {
        var repository = new StubUserRepository
        {
            ActiveUserByEmailOrName = new User
            {
                Id = Guid.NewGuid(),
                FullName = "Customer",
                Email = "customer@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("RightPassword@123"),
                IsActive = true
            }
        };
        var service = new AuthService(repository, new BCryptPasswordHasher(), new StubAuthTokenFactory());

        var result = await service.LoginAsync(new LoginRequest { Email = "customer@test.com", Password = "WrongPassword@123" });

        Assert.False(result.Succeeded);
        Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task Given_PasswordsDoNotMatch_When_ResetPasswordAsync_Then_ReturnsBadRequest()
    {
        var service = new AuthService(new StubUserRepository(), new BCryptPasswordHasher(), new StubAuthTokenFactory());

        var result = await service.ResetPasswordAsync(new ResetPasswordRequest
        {
            Email = "user@test.com",
            NewPassword = "Password@123",
            ConfirmPassword = "Password@456"
        });

        Assert.False(result.Succeeded);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Given_ExistingUser_When_ForgotPasswordAsync_Then_ResetsPassword()
    {
        var repository = new StubUserRepository
        {
            ActiveUser = new User
            {
                Id = Guid.NewGuid(),
                FullName = "Customer",
                Email = "customer@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("OldPassword@123"),
                IsActive = true
            }
        };
        var service = new AuthService(repository, new BCryptPasswordHasher(), new StubAuthTokenFactory());

        var result = await service.ForgotPasswordAsync(new ResetPasswordRequest
        {
            Email = "customer@test.com",
            NewPassword = "NewPassword@123",
            ConfirmPassword = "NewPassword@123"
        });

        Assert.True(result.Succeeded);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.True(BCrypt.Net.BCrypt.Verify("NewPassword@123", repository.ActiveUser!.PasswordHash));
    }

    [Fact]
    public async Task Given_MissingUser_When_ForgotPasswordAsync_Then_ReturnsNotFound()
    {
        var repository = new StubUserRepository();
        var service = new AuthService(repository, new BCryptPasswordHasher(), new StubAuthTokenFactory());

        var result = await service.ForgotPasswordAsync(new ResetPasswordRequest
        {
            Email = "missing@test.com",
            NewPassword = "Password@123",
            ConfirmPassword = "Password@123"
        });

        Assert.False(result.Succeeded);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [Fact]
    public async Task Given_MissingUser_When_ResetPasswordAsync_Then_ReturnsBadRequest()
    {
        var repository = new StubUserRepository();
        var service = new AuthService(repository, new BCryptPasswordHasher(), new StubAuthTokenFactory());

        var result = await service.ResetPasswordAsync(new ResetPasswordRequest
        {
            Email = "missing@test.com",
            NewPassword = "Password@123",
            ConfirmPassword = "Password@123"
        });

        Assert.False(result.Succeeded);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Given_ExistingUser_When_ResetPasswordAsync_Then_ResetsPassword()
    {
        var repository = new StubUserRepository
        {
            ActiveUser = new User
            {
                Id = Guid.NewGuid(),
                FullName = "Customer",
                Email = "customer@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("OldPassword@123"),
                IsActive = true
            }
        };
        var service = new AuthService(repository, new BCryptPasswordHasher(), new StubAuthTokenFactory());

        var result = await service.ResetPasswordAsync(new ResetPasswordRequest
        {
            Email = "customer@test.com",
            NewPassword = "NewPassword@123",
            ConfirmPassword = "NewPassword@123"
        });

        Assert.True(result.Succeeded);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.True(BCrypt.Net.BCrypt.Verify("NewPassword@123", repository.ActiveUser!.PasswordHash));
    }

    [Fact]
    public async Task Given_AdminConfig_When_SeedAdminUserAsync_Then_CreatesAdminUser()
    {
        var repository = new StubUserRepository();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AdminSeed:Email"] = "admin@storehub.com",
                ["AdminSeed:Password"] = "AdminPassword@123",
                ["AdminSeed:FullName"] = "System Admin"
            })
            .Build();
        var service = new AdminSeedService(repository, configuration, new BCryptPasswordHasher());

        await service.SeedAdminUserAsync();

        Assert.Single(repository.AddedUsers);
        Assert.True(repository.AddedUsers[0].IsAdmin);
        Assert.True(repository.AddedUsers[0].IsActive);
        Assert.Equal("admin@storehub.com", repository.AddedUsers[0].Email);
    }

    [Fact]
    public async Task Given_ExistingNonAdmin_When_SeedAdminUserAsync_Then_UpdatesUserToAdmin()
    {
        var repository = new StubUserRepository
        {
            ExistingUser = new User
            {
                Id = Guid.NewGuid(),
                FullName = "Existing Admin",
                Email = "admin@storehub.com",
                PasswordHash = "PlainTextPassword",
                IsAdmin = false,
                IsActive = false,
                CreatedDate = DateTime.UtcNow.AddDays(-1)
            }
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AdminSeed:Email"] = "admin@storehub.com",
                ["AdminSeed:Password"] = "PlainTextPassword",
                ["AdminSeed:FullName"] = "Existing Admin"
            })
            .Build();
        var service = new AdminSeedService(repository, configuration, new BCryptPasswordHasher());

        await service.SeedAdminUserAsync();

        Assert.True(repository.ExistingUser!.IsAdmin);
        Assert.True(repository.ExistingUser.IsActive);
        Assert.NotEqual("PlainTextPassword", repository.ExistingUser.PasswordHash);
    }

    [Fact]
    public async Task Given_IncompleteAdminConfig_When_SeedAdminUserAsync_Then_DoesNothing()
    {
        var repository = new StubUserRepository();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AdminSeed:Email"] = "admin@storehub.com",
                ["AdminSeed:Password"] = "AdminPassword@123"
            })
            .Build();
        var service = new AdminSeedService(repository, configuration, new BCryptPasswordHasher());

        await service.SeedAdminUserAsync();

        Assert.Empty(repository.AddedUsers);
        Assert.Equal(0, repository.SaveChangesCalls);
    }

    [Fact]
    public async Task Given_ExistingConfiguredAdmin_When_SeedAdminUserAsync_Then_DoesNotChangeUser()
    {
        var user = new User
        {
            Id = Guid.NewGuid(), FullName = "System Admin", Email = "admin@storehub.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("AdminPassword@123"), IsAdmin = true, IsActive = true
        };
        var repository = new StubUserRepository { ExistingUser = user };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AdminSeed:Email"] = " admin@storehub.com ",
                ["AdminSeed:Password"] = " AdminPassword@123 ",
                ["AdminSeed:FullName"] = " System Admin "
            })
            .Build();

        await new AdminSeedService(repository, configuration, new BCryptPasswordHasher()).SeedAdminUserAsync();

        Assert.Empty(repository.AddedUsers);
        Assert.Equal(0, repository.SaveChangesCalls);
        Assert.True(user.IsAdmin);
        Assert.True(user.IsActive);
    }

    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "TestJwtSigningKey_StoreHub_Auth_2026_Valid_Secret",
                ["Jwt:Issuer"] = "StoreHub.API",
                ["Jwt:Audience"] = "StoreHub.Client",
                ["Jwt:ExpiryMinutes"] = "60"
            })
            .Build();
    }

    private static IFormFile CreateFormFile(string fileName, string content)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        return new FormFile(new MemoryStream(bytes), 0, bytes.Length, "files", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/png"
        };
    }

    private sealed class StubAuthTokenFactory : IAuthTokenFactory
    {
        public AuthResponse Create(User user)
        {
            return new AuthResponse
            {
                Token = $"token-{user.Id}",
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(60),
                UserId = user.Id.ToString(),
                FullName = user.FullName,
                Email = user.Email,
                IsAdmin = user.IsAdmin
            };
        }
    }

    private sealed class TestWebHostEnvironment : IWebHostEnvironment
    {
        public TestWebHostEnvironment(string webRootPath)
        {
            WebRootPath = webRootPath;
            ContentRootPath = webRootPath;
            WebRootFileProvider = new NullFileProvider();
            ContentRootFileProvider = new NullFileProvider();
            ApplicationName = "StoreHub.Tests";
            EnvironmentName = Environments.Development;
        }

        public string WebRootPath { get; set; }
        public IFileProvider WebRootFileProvider { get; set; }
        public string ContentRootPath { get; set; }
        public IFileProvider ContentRootFileProvider { get; set; }
        public string ApplicationName { get; set; }
        public IFileProvider FileProvider { get; set; } = new NullFileProvider();
        public string EnvironmentName { get; set; }
    }

    private sealed class StubUserRepository : IUserRepository
    {
        public bool ExistsActiveByEmailResult { get; set; }
        public User? ActiveUserByEmailOrName { get; set; }
        public User? ActiveUser { get; set; }
        public User? ExistingUser { get; set; }
        public IReadOnlyList<User> AllUsers { get; set; } = Array.Empty<User>();
        public List<User> AddedUsers { get; } = new();
        public int SaveChangesCalls { get; private set; }

        public Task<bool> ExistsActiveByEmailAsync(string normalizedEmail)
        {
            return Task.FromResult(ExistsActiveByEmailResult);
        }

        public Task<User?> GetActiveByEmailOrFullNameAsync(string normalizedIdentifier)
        {
            return Task.FromResult(ActiveUserByEmailOrName);
        }

        public Task<User?> GetActiveByEmailAsync(string normalizedEmail)
        {
            return Task.FromResult(ActiveUser);
        }

        public Task<User?> GetByEmailAsync(string normalizedEmail)
        {
            return Task.FromResult(ExistingUser);
        }

        public Task<IReadOnlyList<User>> GetAllUsersOrderedByCreatedDateDescAsync()
        {
            return Task.FromResult(AllUsers);
        }

        public Task<User?> GetActiveUserByIdAsync(Guid userId)
        {
            return Task.FromResult<User?>(null);
        }

        public Task AddAsync(User user)
        {
            AddedUsers.Add(user);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync()
        {
            SaveChangesCalls++;
            return Task.CompletedTask;
        }
    }
}