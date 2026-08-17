namespace StoreHub.Domain.Enums
{
    /// <summary>
    /// Represents the status of an order.
    /// Following SRP by centralizing order status definitions.
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>Order has been created but not yet processed.</summary>
        Pending = 1,

        /// <summary>Order is being processed.</summary>
        Processing = 2,

        /// <summary>Order has been shipped.</summary>
        Shipped = 3,

        /// <summary>Order has been delivered.</summary>
        Delivered = 4,

        /// <summary>Order has been cancelled.</summary>
        Cancelled = 5
    }
}
