using System;
using System.Collections.Generic;

namespace StoreHub.Application.Models.Order
{
    public class OrderCreateRequest
    {
        public Guid UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string ShippingAddress { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string ZipCode { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string PaymentMethod { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public List<OrderItemRequest> Items { get; set; } = new();
    }

    public class OrderItemRequest
    {
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
