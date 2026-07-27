using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StoreHub.Domain.Entities;

namespace StoreHub.Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();

        Task<Product?> GetProductByIdAsync(Guid productId);

        Task<Product> CreateProductAsync(Product product);

        Task<Product> UpdateProductAsync(Product product);

        Task<Product> DeleteProductAsync(Product product);

        Task DeleteProductImagesAsync(Guid productId);
    }
}