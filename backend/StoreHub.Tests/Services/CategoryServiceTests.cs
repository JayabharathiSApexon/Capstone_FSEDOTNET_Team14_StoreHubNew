using AutoMapper;
using StoreHub.Application.Exceptions;
using StoreHub.Application.Mappings;
using StoreHub.Application.Models.Category;
using StoreHub.Application.Services;
using StoreHub.Domain.Entities;

namespace StoreHub.Tests.Services;

public class CategoryServiceTests
{
    [Fact]
    public async Task Given_CategoryRequest_When_CreateCategory_Then_PersistsCategoryAndReturnsResponse()
    {
        var repository = new TestCategoryRepository();
        var service = new CategoryService(repository, new MapperConfiguration(x => x.AddProfile<MappingProfile>()).CreateMapper());

        var result = await service.CreateCategoryAsync(new CategoryRequestModel { Name = "Audio", Description = "Devices" });

        Assert.Equal("Audio", result.Name);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Single(repository.Categories);
    }

    [Fact]
    public async Task Given_NoCategories_When_GetAllCategories_Then_ReturnsEmptyCollection()
    {
        var service = new CategoryService(new TestCategoryRepository(), new MapperConfiguration(x => x.AddProfile<MappingProfile>()).CreateMapper());

        var result = await service.GetAllCategoriesAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task Given_MissingCategory_When_GetCategoryById_Then_ReturnsNull()
    {
        var service = new CategoryService(new TestCategoryRepository(), new MapperConfiguration(x => x.AddProfile<MappingProfile>()).CreateMapper());

        var result = await service.GetCategoryByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task Given_MissingCategory_When_UpdateCategory_Then_ThrowsNotFoundException()
    {
        var service = new CategoryService(new TestCategoryRepository(), new MapperConfiguration(x => x.AddProfile<MappingProfile>()).CreateMapper());

        await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateCategoryAsync(new CategoryRequestModel { Id = Guid.NewGuid() }));
    }

    [Fact]
    public async Task Given_ExistingCategory_When_DeleteCategory_Then_MarksCategoryInactive()
    {
        var category = new Category { Id = Guid.NewGuid(), Name = "Audio", IsActive = true };
        var repository = new TestCategoryRepository();
        repository.Categories.Add(category);
        var service = new CategoryService(repository, new MapperConfiguration(x => x.AddProfile<MappingProfile>()).CreateMapper());

        var result = await service.DeleteCategoryAsync(category.Id);

        Assert.False(result.IsActive);
    }

    [Fact]
    public async Task Given_ExistingCategory_When_UpdateCategory_Then_UpdatesFields()
    {
        var category = new Category { Id = Guid.NewGuid(), Name = "Old", Description = "Old description" };
        var repository = new TestCategoryRepository();
        repository.Categories.Add(category);
        var service = new CategoryService(repository, new MapperConfiguration(x => x.AddProfile<MappingProfile>()).CreateMapper());

        var result = await service.UpdateCategoryAsync(new CategoryRequestModel { Id = category.Id, Name = "New", Description = "New description", IsActive = false });

        Assert.Equal("New", result.Name);
        Assert.Equal("New description", category.Description);
        Assert.False(category.IsActive);
        Assert.NotNull(category.UpdatedDate);
    }

    [Fact]
    public async Task Given_MissingCategory_When_DeleteCategory_Then_ThrowsNotFoundException()
    {
        var service = new CategoryService(new TestCategoryRepository(), new MapperConfiguration(x => x.AddProfile<MappingProfile>()).CreateMapper());

        await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteCategoryAsync(Guid.NewGuid()));
    }
}