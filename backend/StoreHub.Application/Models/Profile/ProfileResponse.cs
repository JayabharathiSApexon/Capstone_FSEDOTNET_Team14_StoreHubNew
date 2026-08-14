namespace StoreHub.Application.Models.Profile
{
    public class ProfileResponse
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public bool IsAdmin { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}