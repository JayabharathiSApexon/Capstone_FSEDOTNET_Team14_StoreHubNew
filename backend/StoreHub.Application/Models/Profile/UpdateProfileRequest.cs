using System.ComponentModel.DataAnnotations;

namespace StoreHub.Application.Models.Profile
{
    public class UpdateProfileRequest
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}