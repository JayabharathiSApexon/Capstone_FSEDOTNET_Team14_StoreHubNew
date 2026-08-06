using StoreHub.Domain.Entities;

namespace StoreHub.Application.Interfaces.Repositories
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartByUserIdAsync(Guid userId);

        Task<Cart> CreateCartAsync(Cart cart);

        Task<Cart> UpdateCartAsync(Cart cart);

        Task<CartItem?> GetCartItemAsync(Guid cartId, Guid productId);

        Task AddCartItemAsync(CartItem cartItem);

        Task UpdateCartItemAsync(CartItem cartItem);

        Task DeleteCartItemAsync(Guid cartItemId);

        Task ClearCartAsync(Guid cartId);
    }
}