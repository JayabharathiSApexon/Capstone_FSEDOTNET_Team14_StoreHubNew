namespace StoreHub.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Pending";

        public string ShippingAddress { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string State { get; set; } = string.Empty;

        public string ZipCode { get; set; } = string.Empty;

        public string PaymentMethod { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public User User { get; set; } = null!;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}