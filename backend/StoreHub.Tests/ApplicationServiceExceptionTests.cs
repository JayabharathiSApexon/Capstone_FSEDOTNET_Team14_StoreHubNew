using AutoMapper;
using StoreHub.Application.Exceptions;
using StoreHub.Application.Interfaces.Repositories;
using StoreHub.Application.Models.Category;
using StoreHub.Application.Models.Product;
using StoreHub.Application.Services;
using StoreHub.Domain.Entities;

namespace StoreHub.Tests;

public class ApplicationServiceExceptionTests
{
    [Fact]
    public async Task ProductService_UpdateProductAsync_ShouldThrowNotFoundException_WhenProductDoesNotExist()
    {
        var repository = new MissingProductRepository();
        var mapper = new MapperConfiguration(_ => { }).CreateMapper();
        var service = new ProductService(repository, mapper);

        var request = new ProductRequestModel
        {
            Id = Guid.NewGuid(),
            Name = "Missing Product",
            CategoryId = Guid.NewGuid(),
            Price = 100,
            StockQuantity = 5,
            IsActive = true
        };

        await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateProductAsync(request));
    }

    [Fact]
    public async Task CategoryService_DeleteCategoryAsync_ShouldThrowNotFoundException_WhenCategoryDoesNotExist()
    {
        var repository = new MissingCategoryRepository();
        var mapper = new MapperConfiguration(_ => { }).CreateMapper();
        var service = new CategoryService(repository, mapper);

        await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteCategoryAsync(Guid.NewGuid()));
    }

    private sealed class MissingProductRepository : IProductRepository
    {
        public Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return Task.FromResult(Enumerable.Empty<Product>());
        }

        public Task<Product?> GetProductByIdAsync(Guid productId)
        {
            return Task.FromResult<Product?>(null);
        }

        public Task<Product> CreateProductAsync(Product product)
        {
            throw new NotImplementedException();
        }

        public Task<Product> UpdateProductAsync(Product product)
        {
            throw new NotImplementedException();
        }

        public Task<Product> DeleteProductAsync(Product product)
        {
            throw new NotImplementedException();
        }

        public Task ReplaceProductImagesAsync(Guid productId, List<ProductImage> newImages)
        {
            throw new NotImplementedException();
        }
    }

    private sealed class MissingCategoryRepository : ICategoryRepository
    {
        public Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return Task.FromResult(Enumerable.Empty<Category>());
        }

        public Task<Category?> GetCategoryByIdAsync(Guid categoryId)
        {
            return Task.FromResult<Category?>(null);
        }

        public Task<Category> CreateCategoryAsync(Category category)
        {
            throw new NotImplementedException();
        }

        public Task<Category> UpdateCategoryAsync(Category category)
        {
            throw new NotImplementedException();
        }

        public Task<Category> DeleteCategoryAsync(Category category)
        {
            throw new NotImplementedException();
        }
    }
}