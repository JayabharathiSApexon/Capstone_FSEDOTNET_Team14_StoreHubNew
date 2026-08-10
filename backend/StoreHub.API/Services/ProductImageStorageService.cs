using Microsoft.AspNetCore.Http;
using StoreHub.API.Services.Interfaces;
using StoreHub.Application.Models.Product;

namespace StoreHub.API.Services
{
    public class ProductImageStorageService : IProductImageStorageService
    {
        private readonly IWebHostEnvironment _environment;

        public ProductImageStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<List<ProductImageRequestModel>> SaveNewImagesAsync(IReadOnlyCollection<IFormFile> images)
        {
            var uploadPath = EnsureUploadDirectory();
            var productImages = new List<ProductImageRequestModel>();

            if (images.Count == 0)
            {
                return productImages;
            }

            var displayOrder = 1;
            foreach (var image in images)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                var filePath = Path.Combine(uploadPath, fileName);

                await using var stream = new FileStream(filePath, FileMode.Create);
                await image.CopyToAsync(stream);

                productImages.Add(new ProductImageRequestModel
                {
                    ImageUrl = $"/uploads/products/{fileName}",
                    IsPrimary = displayOrder == 1,
                    DisplayOrder = displayOrder++
                });
            }

            return productImages;
        }

        public async Task<List<ProductImageRequestModel>> ReplaceImagesAsync(
            IEnumerable<string> existingImageUrls,
            IReadOnlyCollection<IFormFile> newImages)
        {
            DeleteExistingImages(existingImageUrls);
            return await SaveNewImagesAsync(newImages);
        }

        private string EnsureUploadDirectory()
        {
            var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "products");
            Directory.CreateDirectory(uploadsPath);
            return uploadsPath;
        }

        private void DeleteExistingImages(IEnumerable<string> existingImageUrls)
        {
            foreach (var imageUrl in existingImageUrls)
            {
                var relativePath = imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                var absolutePath = Path.Combine(_environment.WebRootPath, relativePath);

                if (File.Exists(absolutePath))
                {
                    File.Delete(absolutePath);
                }
            }
        }
    }
}