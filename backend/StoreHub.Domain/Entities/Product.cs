namespace StoreHub.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }

        public Guid CategoryId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Brand { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public bool IsFeatured { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        // Navigation Properties
        public Category Category { get; set; } = null!;

        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    }
}