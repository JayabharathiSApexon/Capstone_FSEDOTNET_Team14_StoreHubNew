using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using StoreHub.Application.Exceptions;
using StoreHub.Application.Interfaces.Repositories;
using StoreHub.Application.Interfaces.Services;
using StoreHub.Application.Models.Product;
using StoreHub.Domain.Entities;

namespace StoreHub.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductQueryRepository _productQueryRepository;
        private readonly IProductCommandRepository _productCommandRepository;
        private readonly IMapper _mapper;

        public ProductService(
            IProductQueryRepository productQueryRepository,
            IProductCommandRepository productCommandRepository,
            IMapper mapper)
        {
            _productQueryRepository = productQueryRepository;
            _productCommandRepository = productCommandRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductResponseModel>> GetAllProductsAsync()
        {
            var products = await _productQueryRepository.GetAllProductsAsync();

            return _mapper.Map<IEnumerable<ProductResponseModel>>(products);
        }

        public async Task<ProductResponseModel?> GetProductByIdAsync(Guid productId)
        {
            var product = await _productQueryRepository.GetProductByIdAsync(productId);

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

            var createdProduct = await _productCommandRepository.CreateProductAsync(product);

            return _mapper.Map<ProductResponseModel>(createdProduct);
        }

        public async Task<ProductResponseModel> UpdateProductAsync(ProductRequestModel request)
        {
            var product = await _productQueryRepository.GetProductByIdAsync(request.Id);

            if (product == null)
                throw new NotFoundException("Product not found.");

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

                await _productCommandRepository.ReplaceProductImagesAsync(product.Id, productImages);
            }

            var updatedProduct = await _productCommandRepository.UpdateProductAsync(product);

            return _mapper.Map<ProductResponseModel>(updatedProduct);
        }
        public async Task<ProductResponseModel> DeleteProductAsync(Guid productId)
        {
            var product = await _productQueryRepository.GetProductByIdAsync(productId);

            if (product == null)
                throw new NotFoundException("Product not found.");

            var deletedProduct = await _productCommandRepository.DeleteProductAsync(product);

            return _mapper.Map<ProductResponseModel>(deletedProduct);
        }
    }
}