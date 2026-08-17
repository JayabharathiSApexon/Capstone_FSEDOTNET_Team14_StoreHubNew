namespace StoreHub.Application.Interfaces.Services
{
    using StoreHub.Domain.Entities;

    /// <summary>
    /// Interface for managing inventory operations.
    /// Follows Dependency Inversion Principle by abstracting inventory concerns
    /// from order and cart services.
    /// </summary>
    public interface IInventoryService
    {
        /// <summary>
        /// Reserves stock for an order item.
        /// </summary>
        /// <param name="productId">The product ID to reserve stock for</param>
        /// <param name="quantity">The quantity to reserve</param>
        /// <returns>True if reservation successful, false if insufficient stock</returns>
        Task<bool> ReserveStockAsync(Guid productId, int quantity);

        /// <summary>
        /// Releases reserved stock (e.g., when order is cancelled).
        /// </summary>
        /// <param name="productId">The product ID to release stock for</param>
        /// <param name="quantity">The quantity to release</param>
        /// <returns>Task representing the asynchronous operation</returns>
        Task ReleaseStockAsync(Guid productId, int quantity);

        /// <summary>
        /// Restores stock for multiple order items.
        /// </summary>
        /// <param name="orderItems">Collection of order items to restore stock for</param>
        /// <returns>Task representing the asynchronous operation</returns>
        Task RestoreStockAsync(IEnumerable<OrderItem> orderItems);

        /// <summary>
        /// Gets the current stock quantity for a product.
        /// </summary>
        /// <param name="productId">The product ID</param>
        /// <returns>Current stock quantity</returns>
        Task<int> GetStockQuantityAsync(Guid productId);
    }
}
