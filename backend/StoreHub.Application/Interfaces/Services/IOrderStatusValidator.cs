using StoreHub.Domain.Enums;

namespace StoreHub.Application.Interfaces.Services
{
    /// <summary>
    /// Interface for validating order status transitions.
    /// Follows SRP by isolating status validation logic.
    /// Follows OCP by allowing extension without modification.
    /// </summary>
    public interface IOrderStatusValidator
    {
        /// <summary>
        /// Validates if a status transition is allowed.
        /// </summary>
        /// <param name="currentStatus">The current order status</param>
        /// <param name="newStatus">The desired new order status</param>
        /// <returns>True if transition is valid, false otherwise</returns>
        bool IsValidTransition(OrderStatus currentStatus, OrderStatus newStatus);

        /// <summary>
        /// Gets all valid next statuses for a given current status.
        /// </summary>
        /// <param name="currentStatus">The current order status</param>
        /// <returns>Collection of valid next statuses</returns>
        IEnumerable<OrderStatus> GetValidNextStatuses(OrderStatus currentStatus);
    }
}
