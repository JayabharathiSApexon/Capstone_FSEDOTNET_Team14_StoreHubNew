using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using StoreHub.API.Common.Services;
using StoreHub.API.Controllers;
using StoreHub.API.Models.Product;
using StoreHub.Application.Interfaces.Services;
using StoreHub.Application.Models.Product;

namespace StoreHub.Tests;

public class ImageStorageServiceTests
{
    [Fact]
    public async Task SaveImagesAsync_WhenImagesAreNull_ReturnsEmptyList()
    {
        var service = new ImageStorageService(new TestWebHostEnvironment());

        var result = await service.SaveImagesAsync(null!);

        Assert.Empty(result);
    }

    [Fact]
    public async Task SaveImagesAsync_WhenImagesAreProvided_StoresFilesAndSetsPrimaryFlag()
    {
        var environment = new TestWebHostEnvironment();
        var service = new ImageStorageService(environment);
        var files = new List<IFormFile>
        {
            CreateFormFile("first.png", "first-content"),
            CreateFormFile("second.png", "second-content")
        };

        var result = await service.SaveImagesAsync(files);

        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].DisplayOrder);
        Assert.Equal(2, result[1].DisplayOrder);
        Assert.True(result[0].IsPrimary);
        Assert.False(result[1].IsPrimary);
        Assert.StartsWith("/uploads/products/", result[0].ImageUrl);
        Assert.StartsWith("/uploads/products/", result[1].ImageUrl);
        Assert.True(File.Exists(Path.Combine(environment.WebRootPath, "uploads", "products", Path.GetFileName(result[0].ImageUrl))));
        Assert.True(File.Exists(Path.Combine(environment.WebRootPath, "uploads", "products", Path.GetFileName(result[1].ImageUrl))));
    }

    [Fact]
    public async Task SaveImagesAsync_WhenImagesAreEmpty_ReturnsEmptyList()
    {
        var service = new ImageStorageService(new TestWebHostEnvironment());

        var result = await service.SaveImagesAsync(new List<IFormFile>());

        Assert.Empty(result);
    }

    [Fact]
    public async Task DeleteImagesAsync_WhenImagesAreNull_DoesNothing()
    {
        var service = new ImageStorageService(new TestWebHostEnvironment());

        await service.DeleteImagesAsync(null!);
    }

    [Fact]
    public async Task DeleteImagesAsync_WhenFilesExist_DeletesThem()
    {
        var environment = new TestWebHostEnvironment();
        var service = new ImageStorageService(environment);
        var folder = Path.Combine(environment.WebRootPath, "uploads", "products");
        Directory.CreateDirectory(folder);
        var fileName = "remove-me.png";
        var fullPath = Path.Combine(folder, fileName);
        await File.WriteAllTextAsync(fullPath, "content");

        await service.DeleteImagesAsync(new[]
        {
            new ProductImageResponseModel
            {
                ImageUrl = "/uploads/products/" + fileName
            }
        });

        Assert.False(File.Exists(fullPath));
    }

    [Fact]
    public async Task DeleteImagesAsync_WhenFileDoesNotExist_DoesNothing()
    {
        var service = new ImageStorageService(new TestWebHostEnvironment());

        await service.DeleteImagesAsync(new[] { new ProductImageResponseModel { ImageUrl = "/uploads/products/missing.png" } });
    }

    [Fact]
    public async Task ProductController_GetProductById_WhenMissing_ReturnsNotFound()
    {
        var controller = new ProductController(
            new FakeProductService(),
            new AutoMapper.MapperConfiguration(cfg => { }).CreateMapper(),
            new FakeImageStorageService());

        var result = await controller.GetProductById(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task ProductController_GetAllProducts_WhenSuccessful_ReturnsOk()
    {
        var result = await new ProductController(new FakeProductService(), new AutoMapper.MapperConfiguration(cfg => { }).CreateMapper(), new FakeImageStorageService()).GetAllProducts();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }

    [Fact]
    public async Task ProductController_GetAllProducts_WhenServiceFails_ReturnsInternalServerError()
    {
        var result = await new ProductController(new FakeProductService { ThrowOnGetAll = true }, new AutoMapper.MapperConfiguration(cfg => { }).CreateMapper(), new FakeImageStorageService()).GetAllProducts();

        var error = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, error.StatusCode);
    }

    [Fact]
    public async Task ProductController_GetProductById_WhenServiceFails_ReturnsInternalServerError()
    {
        var service = new FakeProductService { ThrowOnGet = true };
        var controller = new ProductController(service, new AutoMapper.MapperConfiguration(cfg => { }).CreateMapper(), new FakeImageStorageService());

        var result = await controller.GetProductById(Guid.NewGuid());

        var error = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, error.StatusCode);
    }

    [Fact]
    public async Task ProductController_CreateProduct_WhenValidRequest_ReturnsCreatedAtAction()
    {
        var service = new FakeProductService();
        var imageStorage = new FakeImageStorageService();
        var controller = new ProductController(
            service,
            new AutoMapper.MapperConfiguration(cfg => cfg.CreateMap<CreateProductRequest, ProductRequestModel>().ForMember(dest => dest.Images, opt => opt.Ignore())).CreateMapper(),
            imageStorage);

        var request = new CreateProductRequest
        {
            Name = "Laptop",
            CategoryId = Guid.NewGuid(),
            Price = 999m,
            StockQuantity = 8,
            Images = new List<IFormFile> { CreateFormFile("laptop.png", "content") }
        };

        var result = await controller.CreateProduct(request);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(ProductController.GetProductById), created.ActionName);
        Assert.True(imageStorage.SaveCalled);
        Assert.Equal("Laptop", service.LastCreatedRequest?.Name);
    }

    [Fact]
    public async Task ProductController_UpdateProduct_WhenIdMismatch_ReturnsBadRequest()
    {
        var controller = new ProductController(
            new FakeProductService(),
            new AutoMapper.MapperConfiguration(cfg => cfg.CreateMap<UpdateProductRequest, ProductRequestModel>().ForMember(dest => dest.Images, opt => opt.Ignore())).CreateMapper(),
            new FakeImageStorageService());

        var request = new UpdateProductRequest
        {
            Id = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Name = "Updated",
            Price = 45m,
            StockQuantity = 3
        };

        var result = await controller.UpdateProduct(Guid.NewGuid(), request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Product ID mismatch.", badRequest.Value);
    }

    [Fact]
    public async Task ProductController_UpdateProduct_WhenProductMissing_ReturnsNotFound()
    {
        var request = new UpdateProductRequest { Id = Guid.NewGuid(), CategoryId = Guid.NewGuid(), Name = "Updated", Price = 1m, StockQuantity = 1 };
        var result = await new ProductController(new FakeProductService(), new AutoMapper.MapperConfiguration(cfg => cfg.CreateMap<UpdateProductRequest, ProductRequestModel>().ForMember(dest => dest.Images, opt => opt.Ignore())).CreateMapper(), new FakeImageStorageService()).UpdateProduct(request.Id, request);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task ProductController_UpdateProduct_WhenImagesProvided_DeletesOldAndSavesNewImages()
    {
        var service = new FakeProductService { ProductToReturn = new ProductResponseModel { Id = Guid.NewGuid(), Images = new List<ProductImageResponseModel>() } };
        var imageStorage = new FakeImageStorageService();
        var controller = new ProductController(service, new AutoMapper.MapperConfiguration(cfg => cfg.CreateMap<UpdateProductRequest, ProductRequestModel>().ForMember(dest => dest.Images, opt => opt.Ignore())).CreateMapper(), imageStorage);
        var request = new UpdateProductRequest { Id = service.ProductToReturn.Id, CategoryId = Guid.NewGuid(), Name = "Updated", Price = 1m, StockQuantity = 1, Images = new List<IFormFile> { CreateFormFile("updated.png", "content") } };

        var result = await controller.UpdateProduct(request.Id, request);

        Assert.IsType<OkObjectResult>(result);
        Assert.True(imageStorage.DeleteCalled);
        Assert.True(imageStorage.SaveCalled);
    }

    [Fact]
    public async Task ProductController_UpdateProduct_WhenImagesAreEmpty_DoesNotTouchImageStorage()
    {
        var service = new FakeProductService { ProductToReturn = new ProductResponseModel { Id = Guid.NewGuid() } };
        var imageStorage = new FakeImageStorageService();
        var controller = new ProductController(service, new AutoMapper.MapperConfiguration(cfg => cfg.CreateMap<UpdateProductRequest, ProductRequestModel>().ForMember(dest => dest.Images, opt => opt.Ignore())).CreateMapper(), imageStorage);
        var request = new UpdateProductRequest { Id = service.ProductToReturn!.Id, CategoryId = Guid.NewGuid(), Name = "Updated", Price = 1m, StockQuantity = 1, Images = new List<IFormFile>() };

        Assert.IsType<OkObjectResult>(await controller.UpdateProduct(request.Id, request));
        Assert.False(imageStorage.DeleteCalled);
        Assert.False(imageStorage.SaveCalled);
    }

    [Fact]
    public async Task ProductController_CreateProduct_WhenImageStorageFails_ReturnsBadRequest()
    {
        var imageStorage = new FakeImageStorageService { ThrowOnSave = true };
        var controller = new ProductController(new FakeProductService(), new AutoMapper.MapperConfiguration(cfg => cfg.CreateMap<CreateProductRequest, ProductRequestModel>().ForMember(dest => dest.Images, opt => opt.Ignore())).CreateMapper(), imageStorage);

        var result = await controller.CreateProduct(new CreateProductRequest { Name = "Laptop", CategoryId = Guid.NewGuid(), Price = 1m, StockQuantity = 1, Images = new List<IFormFile>() });

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task ProductController_DeleteProduct_WhenProductExists_DeletesImagesAndReturnsOk()
    {
        var service = new FakeProductService
        {
            ProductToReturn = new ProductResponseModel
            {
                Id = Guid.NewGuid(),
                Name = "Camera",
                Images = new List<ProductImageResponseModel>
                {
                    new() { ImageUrl = "/uploads/products/camera.png" }
                }
            }
        };
        var imageStorage = new FakeImageStorageService();
        var controller = new ProductController(
            service,
            new AutoMapper.MapperConfiguration(cfg => { }).CreateMapper(),
            imageStorage);

        var result = await controller.DeleteProduct(service.ProductToReturn!.Id);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
        Assert.True(imageStorage.DeleteCalled);
    }

    [Fact]
    public async Task ProductController_DeleteProduct_WhenProductMissing_ReturnsNotFound()
    {
        var result = await new ProductController(new FakeProductService(), new AutoMapper.MapperConfiguration(cfg => { }).CreateMapper(), new FakeImageStorageService()).DeleteProduct(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }

    private static IFormFile CreateFormFile(string fileName, string content)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(bytes);
        return new FormFile(stream, 0, bytes.Length, "files", fileName);
    }

    private sealed class TestWebHostEnvironment : IWebHostEnvironment
    {
        public TestWebHostEnvironment()
        {
            WebRootPath = Path.Combine(Path.GetTempPath(), "storehub-tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(WebRootPath);
        }

        public string WebRootPath { get; set; }
        public string ApplicationName { get; set; } = "StoreHub.Tests";
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string ContentRootPath { get; set; } = Path.GetTempPath();
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        public string EnvironmentName { get; set; } = Environments.Development;
    }

    private sealed class FakeImageStorageService : IImageStorageService
    {
        public bool SaveCalled { get; private set; }
        public bool DeleteCalled { get; private set; }
        public bool ThrowOnSave { get; set; }

        public Task<List<ProductImageRequestModel>> SaveImagesAsync(List<IFormFile> images)
        {
            SaveCalled = true;
            if (ThrowOnSave)
                throw new InvalidOperationException("image failure");

            return Task.FromResult(images
                .Select((image, index) => new ProductImageRequestModel
                {
                    ImageUrl = "/uploads/products/" + image.FileName,
                    IsPrimary = index == 0,
                    DisplayOrder = index + 1
                })
                .ToList());
        }

        public Task DeleteImagesAsync(IEnumerable<ProductImageResponseModel> images)
        {
            DeleteCalled = true;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeProductService : IProductService
    {
        public ProductResponseModel? ProductToReturn { get; set; }
        public ProductRequestModel? LastCreatedRequest { get; private set; }
        public bool ThrowOnGetAll { get; set; }
        public bool ThrowOnGet { get; set; }

        public Task<IEnumerable<ProductResponseModel>> GetAllProductsAsync()
        {
            if (ThrowOnGetAll)
                throw new InvalidOperationException("service failure");

            return Task.FromResult<IEnumerable<ProductResponseModel>>(new[]
            {
                new ProductResponseModel { Id = Guid.NewGuid(), Name = "Sample" }
            });
        }

        public Task<ProductResponseModel?> GetProductByIdAsync(Guid productId)
        {
            if (ThrowOnGet)
                throw new InvalidOperationException("service failure");

            return Task.FromResult(ProductToReturn?.Id == productId ? ProductToReturn : null);
        }

        public Task<ProductResponseModel> CreateProductAsync(ProductRequestModel request)
        {
            LastCreatedRequest = request;
            return Task.FromResult(new ProductResponseModel
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                CategoryId = request.CategoryId,
                Price = request.Price,
                StockQuantity = request.StockQuantity
            });
        }

        public Task<ProductResponseModel> UpdateProductAsync(ProductRequestModel request)
        {
            return Task.FromResult(new ProductResponseModel
            {
                Id = request.Id,
                Name = request.Name,
                CategoryId = request.CategoryId,
                Price = request.Price,
                StockQuantity = request.StockQuantity
            });
        }

        public Task<ProductResponseModel> DeleteProductAsync(Guid productId)
        {
            return Task.FromResult(new ProductResponseModel
            {
                Id = productId,
                Name = "Deleted"
            });
        }
    }
}
