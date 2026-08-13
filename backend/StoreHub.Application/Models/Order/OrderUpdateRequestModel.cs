namespace StoreHub.Application.Models.Order
{
    public class OrderUpdateRequestModel
    {
        public Guid OrderId { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}