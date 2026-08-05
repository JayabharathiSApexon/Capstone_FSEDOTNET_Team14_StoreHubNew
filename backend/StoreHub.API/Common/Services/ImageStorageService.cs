using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using StoreHub.API.Models.Product;
using StoreHub.Application.Models.Product;

namespace StoreHub.API.Common.Services
{
    public class ImageStorageService : IImageStorageService
    {
        private readonly IWebHostEnvironment _environment;

        public ImageStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<List<ProductImageRequestModel>> SaveImagesAsync(List<IFormFile> images)
        {
            var productImages = new List<ProductImageRequestModel>();

            if (images == null || !images.Any())
            {
                return productImages;
            }

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "products");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            int displayOrder = 1;

            foreach (var image in images)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";

                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);

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

        public async Task DeleteImagesAsync(IEnumerable<ProductImageResponseModel> images)
        {
            if (images == null)
            {
                return;
            }

            foreach (var image in images)
            {
                var filePath = Path.Combine(_environment.WebRootPath,
                    image.ImageUrl
                        .TrimStart('/')
                        .Replace('/', Path.DirectorySeparatorChar));

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }

            await Task.CompletedTask;
        }
    }
}