using StoreHub.Application.Interfaces.Services;
using StoreHub.Domain.Enums;

namespace StoreHub.Application.Services
{
    /// <summary>
    /// Validates order status transitions.
    /// Implements OCP by defining transition rules that can be extended without modifying existing code.
    /// Implements SRP by focusing solely on status transition validation.
    /// </summary>
    public class OrderStatusValidator : IOrderStatusValidator
    {
        /// <summary>
        /// Defines the valid status transitions for an order.
        /// Maps current status to a collection of valid next statuses.
        /// </summary>
        private static readonly Dictionary<OrderStatus, HashSet<OrderStatus>> ValidTransitions = 
            new()
            {
                { 
                    OrderStatus.Pending, 
                    new HashSet<OrderStatus> 
                    { 
                        OrderStatus.Processing, 
                        OrderStatus.Cancelled 
                    } 
                },
                { 
                    OrderStatus.Processing, 
                    new HashSet<OrderStatus> 
                    { 
                        OrderStatus.Shipped, 
                        OrderStatus.Cancelled 
                    } 
                },
                { 
                    OrderStatus.Shipped, 
                    new HashSet<OrderStatus> 
                    { 
                        OrderStatus.Delivered 
                    } 
                },
                { 
                    OrderStatus.Delivered, 
                    new HashSet<OrderStatus>() // Terminal state - no transitions
                },
                { 
                    OrderStatus.Cancelled, 
                    new HashSet<OrderStatus>() // Terminal state - no transitions
                }
            };

        /// <summary>
        /// Validates if a transition from current status to new status is allowed.
        /// </summary>
        public bool IsValidTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            if (currentStatus == newStatus)
                return false; // Cannot transition to the same status

            if (!ValidTransitions.TryGetValue(currentStatus, out var validNextStatuses))
                return false; // Current status not found in transition map

            return validNextStatuses.Contains(newStatus);
        }

        /// <summary>
        /// Gets all valid next statuses for a given current status.
        /// </summary>
        public IEnumerable<OrderStatus> GetValidNextStatuses(OrderStatus currentStatus)
        {
            if (ValidTransitions.TryGetValue(currentStatus, out var validNextStatuses))
                return validNextStatuses;

            return Enumerable.Empty<OrderStatus>();
        }
    }
}
