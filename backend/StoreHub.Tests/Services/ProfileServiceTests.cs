using StoreHub.Application.Models.Profile;
using StoreHub.Application.Services;
using StoreHub.Domain.Entities;

namespace StoreHub.Tests.Services;

public class ProfileServiceTests
{
    [Fact]
    public async Task Given_ActiveUser_When_GetProfile_Then_ReturnsProfileDetails()
    {
        var user = new User { Id = Guid.NewGuid(), FullName = "Test User", Email = "user@example.com", PhoneNumber = "123" };
        var repository = new TestUserRepository { User = user };
        var service = new ProfileService(repository);

        var result = await service.GetProfileAsync(user.Id);

        Assert.NotNull(result);
        Assert.Equal(user.Email, result!.Email);
        Assert.Equal(user.FullName, result.FullName);
    }

    [Fact]
    public async Task Given_MissingUser_When_GetProfile_Then_ReturnsNull()
    {
        var result = await new ProfileService(new TestUserRepository()).GetProfileAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task Given_MissingUser_When_UpdateProfile_Then_ReturnsUserNotFound()
    {
        var service = new ProfileService(new TestUserRepository());

        var result = await service.UpdateProfileAsync(Guid.NewGuid(), new UpdateProfileRequest { FullName = "User", Email = "user@example.com" });

        Assert.Equal(UpdateProfileResult.UserNotFound, result);
    }

    [Fact]
    public async Task Given_NewEmailAndWhitespace_When_UpdateProfile_Then_NormalizesAndSaves()
    {
        var user = new User { Id = Guid.NewGuid(), FullName = "Old", Email = "old@example.com" };
        var repository = new TestUserRepository { User = user };
        var service = new ProfileService(repository);

        var result = await service.UpdateProfileAsync(user.Id, new UpdateProfileRequest { FullName = " New Name ", Email = " NEW@Example.COM ", PhoneNumber = " 555 " });

        Assert.Equal(UpdateProfileResult.Success, result);
        Assert.Equal("New Name", user.FullName);
        Assert.Equal("new@example.com", user.Email);
        Assert.Equal("555", user.PhoneNumber);
        Assert.Equal(1, repository.SaveChangesCalls);
    }

    [Fact]
    public async Task Given_EmailOwnedByAnotherUser_When_UpdateProfile_Then_ReturnsEmailAlreadyExists()
    {
        var user = new User { Id = Guid.NewGuid(), Email = "old@example.com" };
        var repository = new TestUserRepository { User = user, ExistingUser = new User { Id = Guid.NewGuid() } };
        var service = new ProfileService(repository);

        var result = await service.UpdateProfileAsync(user.Id, new UpdateProfileRequest { FullName = "User", Email = "taken@example.com" });

        Assert.Equal(UpdateProfileResult.EmailAlreadyExists, result);
        Assert.Equal(0, repository.SaveChangesCalls);
    }

    [Fact]
    public async Task Given_SameEmailAndNullPhone_When_UpdateProfile_Then_SavesNormalizedValues()
    {
        var user = new User { Id = Guid.NewGuid(), FullName = "Old", Email = "old@example.com" };
        var repository = new TestUserRepository { User = user, ExistingUser = user };

        var result = await new ProfileService(repository).UpdateProfileAsync(user.Id, new UpdateProfileRequest
        {
            FullName = " New Name ",
            Email = " OLD@EXAMPLE.COM ",
            PhoneNumber = null!
        });

        Assert.Equal(UpdateProfileResult.Success, result);
        Assert.Equal(string.Empty, user.PhoneNumber);
        Assert.Equal(1, repository.SaveChangesCalls);
    }
}