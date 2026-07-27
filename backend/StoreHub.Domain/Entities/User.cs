namespace StoreHub.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public bool IsAdmin { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public ICollection<Cart> Carts { get; set; } = new List<Cart>();

        public ICollection<Order> Orders { get; set; } = new List<Order>();

        public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    }
}