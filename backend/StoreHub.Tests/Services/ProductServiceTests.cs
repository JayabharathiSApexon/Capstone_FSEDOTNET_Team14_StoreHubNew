using AutoMapper;
using StoreHub.Application.Exceptions;
using StoreHub.Application.Mappings;
using StoreHub.Application.Models.Product;
using StoreHub.Application.Services;
using StoreHub.Domain.Entities;

namespace StoreHub.Tests.Services;

public class ProductServiceTests
{
    private static IMapper CreateMapper() => new MapperConfiguration(x => x.AddProfile<MappingProfile>()).CreateMapper();

    [Fact]
    public async Task Given_ProductRequest_When_CreateProduct_Then_CreatesProductAndImages()
    {
        var repository = new TestProductRepository();
        var service = new ProductService(repository, CreateMapper());
        var request = new ProductRequestModel { Name = "Mouse", CategoryId = Guid.NewGuid(), Price = 20m, StockQuantity = 5, Images = new List<ProductImageRequestModel> { new() { ImageUrl = "/mouse.png", IsPrimary = true, DisplayOrder = 1 } } };

        var result = await service.CreateProductAsync(request);

        Assert.Equal("Mouse", result.Name);
        Assert.Single(repository.Products[0].ProductImages);
        Assert.Equal("/mouse.png", repository.Products[0].ProductImages.First().ImageUrl);
    }

    [Fact]
    public async Task Given_MissingProduct_When_DeleteProduct_Then_ThrowsNotFoundException()
    {
        var service = new ProductService(new TestProductRepository(), CreateMapper());

        await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteProductAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task Given_ProductUpdateWithImages_When_UpdateProduct_Then_ReplacesImagesAndFields()
    {
        var category = new Category { Id = Guid.NewGuid(), Name = "Accessories" };
        var product = new Product { Id = Guid.NewGuid(), Name = "Old", CategoryId = category.Id, Category = category, ProductImages = new List<ProductImage>() };
        var repository = new TestProductRepository();
        repository.Products.Add(product);
        var service = new ProductService(repository, CreateMapper());

        var result = await service.UpdateProductAsync(new ProductRequestModel { Id = product.Id, Name = "New", CategoryId = category.Id, Price = 30m, StockQuantity = 8, IsActive = true, Images = new List<ProductImageRequestModel> { new() { ImageUrl = "/new.png" } } });

        Assert.Equal("New", result.Name);
        Assert.Equal(30m, product.Price);
        Assert.Single(repository.ReplacedImages);
        Assert.True(repository.ReplacedImages[0].IsPrimary);
    }
}