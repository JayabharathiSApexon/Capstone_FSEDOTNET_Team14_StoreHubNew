using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace StoreHub.API.Models.Product
{
    public class UpdateProductRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Brand { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        public bool IsFeatured { get; set; }

        public bool IsActive { get; set; }

        public List<IFormFile>? Images { get; set; }
    }
}