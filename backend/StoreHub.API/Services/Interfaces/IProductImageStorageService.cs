using Microsoft.AspNetCore.Http;
using StoreHub.Application.Models.Product;

namespace StoreHub.API.Services.Interfaces
{
    public interface IProductImageStorageService
    {
        Task<List<ProductImageRequestModel>> SaveNewImagesAsync(IReadOnlyCollection<IFormFile> images);

        Task<List<ProductImageRequestModel>> ReplaceImagesAsync(
            IEnumerable<string> existingImageUrls,
            IReadOnlyCollection<IFormFile> newImages);
    }
}