namespace StoreHub.Application.Models.Cart
{
    public class AddToCartRequestModel
    {
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }
    }
}