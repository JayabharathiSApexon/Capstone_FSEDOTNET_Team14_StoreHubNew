using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoreHub.Application.Interfaces.Repositories;
using StoreHub.Domain.Entities;
using StoreHub.Infrastructure.Data;

namespace StoreHub.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(product => product.Category)
                .Include(product => product.ProductImages)
                .Where(product => product.IsActive)
                .OrderBy(product => product.Name)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(Guid productId)
        {
            return await _context.Products
                .Include(product => product.Category)
                .Include(product => product.ProductImages)
                .FirstOrDefaultAsync(product =>
                    product.Id == productId &&
                    product.IsActive);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return product;
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);

            await _context.SaveChangesAsync();

            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .FirstAsync(p => p.Id == product.Id);
        }

        public async Task<Product> DeleteProductAsync(Product product)
        {
            product.IsActive = false;
            product.UpdatedDate = DateTime.UtcNow;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return product;
        }

        public async Task DeleteProductImagesAsync(Guid productId)
        {
            var images = await _context.ProductImages
                .Where(image => image.ProductId == productId)
                .ToListAsync();

            _context.ProductImages.RemoveRange(images);
        }
    }
}