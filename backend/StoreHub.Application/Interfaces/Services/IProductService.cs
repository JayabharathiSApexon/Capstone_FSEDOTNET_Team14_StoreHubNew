using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StoreHub.Application.Models.Product;

namespace StoreHub.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponseModel>> GetAllProductsAsync();

        Task<ProductResponseModel?> GetProductByIdAsync(Guid productId);

        Task<ProductResponseModel> CreateProductAsync(ProductRequestModel request);

        Task<ProductResponseModel> UpdateProductAsync(ProductRequestModel request);

        Task<ProductResponseModel> DeleteProductAsync(Guid productId);
    }
}