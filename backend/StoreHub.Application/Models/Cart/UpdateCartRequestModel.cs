namespace StoreHub.Application.Models.Cart
{
    public class UpdateCartRequestModel
    {
        public Guid CartItemId { get; set; }

        public int Quantity { get; set; }
    }
}