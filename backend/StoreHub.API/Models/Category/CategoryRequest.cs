using System.ComponentModel.DataAnnotations;

namespace StoreHub.API.Models.Category
{
    public class CategoryRequest
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}