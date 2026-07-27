using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace StoreHub.API.Models.Product
{
    public class CreateProductRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public Guid CategoryId { get; set; }

        public string Description { get; set; } = string.Empty;

        public string Brand { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        public bool IsFeatured { get; set; }

        public bool IsActive { get; set; } = true;

        public List<IFormFile> Images { get; set; } = new();
    }
}