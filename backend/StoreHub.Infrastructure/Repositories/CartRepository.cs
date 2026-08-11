using Microsoft.EntityFrameworkCore;
using StoreHub.Application.Interfaces.Repositories;
using StoreHub.Domain.Entities;
using StoreHub.Infrastructure.Data;

namespace StoreHub.Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetCartByUserIdAsync(Guid userId)
        {
            return await _context.Carts
                .Include(x => x.CartItems)
                    .ThenInclude(x => x.Product)
                        .ThenInclude(x => x.ProductImages)
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<Cart> CreateCartAsync(Cart cart)
        {
            await _context.Carts.AddAsync(cart);

            await _context.SaveChangesAsync();

            return cart;
        }

        public async Task<Cart> UpdateCartAsync(Cart cart)
        {
            _context.Carts.Update(cart);

            await _context.SaveChangesAsync();

            return cart;
        }

        public async Task<CartItem?> GetCartItemAsync(Guid cartId, Guid productId)
        {
            return await _context.CartItems
                .FirstOrDefaultAsync(x =>
                    x.CartId == cartId &&
                    x.ProductId == productId);
        }

        public async Task AddCartItemAsync(CartItem cartItem)
        {
            await _context.CartItems.AddAsync(cartItem);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateCartItemAsync(CartItem cartItem)
        {
            _context.CartItems.Update(cartItem);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteCartItemAsync(Guid cartItemId)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(x => x.Id == cartItemId);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);

                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(Guid cartId)
        {
            var items = await _context.CartItems
                .Where(x => x.CartId == cartId)
                .ToListAsync();

            if (items.Any())
            {
                _context.CartItems.RemoveRange(items);

                await _context.SaveChangesAsync();
            }
        }
    }
}