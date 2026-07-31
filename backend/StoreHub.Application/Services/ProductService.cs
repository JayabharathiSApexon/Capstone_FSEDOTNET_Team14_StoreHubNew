using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using StoreHub.Application.Interfaces.Repositories;
using StoreHub.Application.Interfaces.Services;
using StoreHub.Application.Models.Product;
using StoreHub.Domain.Entities;

namespace StoreHub.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductService(
            IProductRepository productRepository,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductResponseModel>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllProductsAsync();

            return _mapper.Map<IEnumerable<ProductResponseModel>>(products);
        }

        public async Task<ProductResponseModel?> GetProductByIdAsync(Guid productId)
        {
            var product = await _productRepository.GetProductByIdAsync(productId);

            if (product == null)
                return null;

            return _mapper.Map<ProductResponseModel>(product);
        }

        public async Task<ProductResponseModel> CreateProductAsync(ProductRequestModel request)
        {
            var product = _mapper.Map<Product>(request);

            product.Id = Guid.NewGuid();
            product.CreatedDate = DateTime.UtcNow;

            foreach (var image in request.Images)
            {
                product.ProductImages.Add(new ProductImage
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    ImageUrl = image.ImageUrl,
                    IsPrimary = image.IsPrimary,
                    DisplayOrder = image.DisplayOrder,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                });
            }

            var createdProduct = await _productRepository.CreateProductAsync(product);

            return _mapper.Map<ProductResponseModel>(createdProduct);
        }

        public async Task<ProductResponseModel> UpdateProductAsync(ProductRequestModel request)
        {
            var product = await _productRepository.GetProductByIdAsync(request.Id);

            if (product == null)
                throw new Exception("Product not found.");

            product.Name = request.Name;
            product.CategoryId = request.CategoryId;
            product.Description = request.Description;
            product.Brand = request.Brand;
            product.Price = request.Price;
            product.StockQuantity = request.StockQuantity;
            product.IsFeatured = request.IsFeatured;
            product.IsActive = request.IsActive;
            product.UpdatedDate = DateTime.UtcNow;

            if (request.Images != null && request.Images.Any())
            {
                int displayOrder = 1;

                var productImages = request.Images
                    .Select(image => new ProductImage
                    {
                        Id = Guid.NewGuid(),
                        ProductId = product.Id,
                        ImageUrl = image.ImageUrl,
                        IsPrimary = displayOrder == 1,
                        DisplayOrder = displayOrder++,
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow
                    })
                    .ToList();

                await _productRepository.ReplaceProductImagesAsync(product.Id, productImages);
            }

            var updatedProduct = await _productRepository.UpdateProductAsync(product);

            return _mapper.Map<ProductResponseModel>(updatedProduct);
        }
        public async Task<ProductResponseModel> DeleteProductAsync(Guid productId)
        {
            var product = await _productRepository.GetProductByIdAsync(productId);

            if (product == null)
                throw new Exception("Product not found.");

            var deletedProduct = await _productRepository.DeleteProductAsync(product);

            return _mapper.Map<ProductResponseModel>(deletedProduct);
        }
    }
}