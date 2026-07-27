using System;
using System.Collections.Generic;
using StoreHub.Application.Models.Product;

namespace StoreHub.Application.Models.Product
{
    public class ProductRequestModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public Guid CategoryId { get; set; }

        public string Description { get; set; } = string.Empty;

        public string Brand { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public bool IsFeatured { get; set; }

        public bool IsActive { get; set; }

        public List<ProductImageRequestModel> Images { get; set; } = new();
    }
}