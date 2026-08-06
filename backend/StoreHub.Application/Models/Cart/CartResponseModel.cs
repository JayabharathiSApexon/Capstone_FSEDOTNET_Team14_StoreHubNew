namespace StoreHub.Application.Models.Cart
{
    public class CartResponseModel
    {
        public Guid CartId { get; set; }

        public decimal SubTotal { get; set; }

        public decimal Shipping { get; set; }

        public decimal Total { get; set; }

        public List<CartItemResponseModel> Items { get; set; } = new();
    }
}