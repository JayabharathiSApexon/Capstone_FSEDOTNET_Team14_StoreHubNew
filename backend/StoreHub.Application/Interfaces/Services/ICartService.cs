using StoreHub.Application.Models.Cart;

namespace StoreHub.Application.Interfaces.Services
{
    public interface ICartService
    {
        Task<CartResponseModel> GetCartAsync(Guid userId);

        Task<CartResponseModel> AddToCartAsync(Guid userId, AddToCartRequestModel request);

        Task<CartResponseModel> UpdateCartItemAsync(Guid userId, UpdateCartRequestModel request);

        Task RemoveCartItemAsync(Guid userId, Guid cartItemId);

        Task ClearCartAsync(Guid userId);
    }
}