using Microsoft.AspNetCore.Http;
using StoreHub.API.Models.Product;
using StoreHub.Application.Models.Product;

namespace StoreHub.API.Common.Services
{
    public interface IImageStorageService
    {
        Task<List<ProductImageRequestModel>> SaveImagesAsync(List<IFormFile> images);

        Task DeleteImagesAsync(IEnumerable<ProductImageResponseModel> images);
    }
}