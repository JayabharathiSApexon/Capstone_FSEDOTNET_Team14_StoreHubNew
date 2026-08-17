using StoreHub.Application.Interfaces.Repositories;
using StoreHub.Application.Interfaces.Services;
using StoreHub.Domain.Entities;

namespace StoreHub.Application.Services
{
    /// <summary>
    /// Service for managing inventory operations.
    /// Implements SRP by handling all inventory-related concerns separately from order/cart logic.
    /// Implements DIP by providing an abstraction for inventory management.
    /// </summary>
    public class InventoryService : IInventoryService
    {
        private readonly IProductRepository _productRepository;

        public InventoryService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        /// <summary>
        /// Reserves stock for an order item by reducing product stock quantity.
        /// </summary>
        public async Task<bool> ReserveStockAsync(Guid productId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
                throw new ArgumentException($"Product with ID {productId} not found.", nameof(productId));

            if (product.StockQuantity < quantity)
                return false; // Insufficient stock

            product.StockQuantity -= quantity;
            // Ensure stock never goes negative
            if (product.StockQuantity < 0)
                product.StockQuantity = 0;

            product.UpdatedDate = DateTime.UtcNow;
            await _productRepository.UpdateProductAsync(product);

            return true;
        }

        /// <summary>
        /// Releases previously reserved stock (e.g., when order is cancelled).
        /// </summary>
        public async Task ReleaseStockAsync(Guid productId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
                throw new ArgumentException($"Product with ID {productId} not found.", nameof(productId));

            product.StockQuantity += quantity;
            product.UpdatedDate = DateTime.UtcNow;
            await _productRepository.UpdateProductAsync(product);
        }

        /// <summary>
        /// Restores stock for multiple order items.
        /// </summary>
        public async Task RestoreStockAsync(IEnumerable<OrderItem> orderItems)
        {
            if (orderItems == null || !orderItems.Any())
                return;

            foreach (var orderItem in orderItems)
            {
                try
                {
                    await ReleaseStockAsync(orderItem.ProductId, orderItem.Quantity);
                }
                catch (ArgumentException ex)
                {
                    throw new InvalidOperationException(
                        $"Failed to restore stock for product {orderItem.ProductId}. {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Gets the current stock quantity for a product.
        /// </summary>
        public async Task<int> GetStockQuantityAsync(Guid productId)
        {
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
                throw new ArgumentException($"Product with ID {productId} not found.", nameof(productId));

            return product.StockQuantity;
        }
    }
}
