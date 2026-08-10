using StoreHub.Domain.Entities;

namespace StoreHub.Application.Interfaces.Repositories
{
    public interface IProductCommandRepository
    {
        Task<Product> CreateProductAsync(Product product);

        Task<Product> UpdateProductAsync(Product product);

        Task<Product> DeleteProductAsync(Product product);

        Task ReplaceProductImagesAsync(Guid productId, List<ProductImage> newImages);
    }
}