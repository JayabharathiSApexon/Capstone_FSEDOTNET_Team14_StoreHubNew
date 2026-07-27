namespace StoreHub.Domain.Entities
{
    public class ProductImage
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public bool IsPrimary { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public Product Product { get; set; } = null!;
    }
}