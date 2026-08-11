using System;

namespace StoreHub.Application.Models.Order
{
    public class TrackingProductModel
    {
        public Guid ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public string ImageUrl { get; set; } = string.Empty;
    }
}
