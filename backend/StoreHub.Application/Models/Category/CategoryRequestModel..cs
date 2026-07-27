using System;

namespace StoreHub.Application.Models.Category
{
    public class CategoryRequestModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}