using AutoMapper;
using StoreHub.Application.Interfaces.Repositories;
using StoreHub.Application.Interfaces.Services;
using StoreHub.Application.Models.Cart;
using StoreHub.Domain.Entities;

namespace StoreHub.Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository, IMapper mapper)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<CartResponseModel> GetCartAsync(Guid userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);

            if (cart == null)
            {
                return new CartResponseModel();
            }

            return MapCart(cart);
        }

        public async Task<CartResponseModel> AddToCartAsync(Guid userId, AddToCartRequestModel request)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CreatedDate = DateTime.UtcNow
                };

                await _cartRepository.CreateCartAsync(cart);
            }

            var product = await _productRepository.GetProductByIdAsync(request.ProductId);

            if (product == null)
            {
                throw new Exception("Product not found.");
            }

            var existingItem = await _cartRepository.GetCartItemAsync(cart.Id, request.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
                existingItem.UpdatedDate = DateTime.UtcNow;

                await _cartRepository.UpdateCartItemAsync(existingItem);
            }
            else
            {
                await _cartRepository.AddCartItemAsync(
                    new CartItem
                    {
                        Id = Guid.NewGuid(),
                        CartId = cart.Id,
                        ProductId = request.ProductId,
                        Quantity = request.Quantity,
                        CreatedDate = DateTime.UtcNow
                    });
            }

            cart = await _cartRepository.GetCartByUserIdAsync(userId);

            return MapCart(cart!);
        }

        public async Task<CartResponseModel> UpdateCartItemAsync(Guid userId, UpdateCartRequestModel request)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);

            if (cart == null)
            {
                throw new Exception("Cart not found.");
            }

            var item = cart.CartItems.FirstOrDefault(x => x.Id == request.CartItemId);

            if (item == null)
            {
                throw new Exception("Cart item not found.");
            }

            item.Quantity = request.Quantity;
            item.UpdatedDate = DateTime.UtcNow;

            await _cartRepository.UpdateCartItemAsync(item);

            cart = await _cartRepository.GetCartByUserIdAsync(userId);

            return MapCart(cart!);
        }

        public async Task RemoveCartItemAsync(Guid userId, Guid cartItemId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);

            if (cart == null)
            {
                throw new Exception("Cart not found.");
            }

            await _cartRepository.DeleteCartItemAsync(cartItemId);
        }

        public async Task ClearCartAsync(Guid userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);

            if (cart == null)
            {
                return;
            }

            await _cartRepository.ClearCartAsync(cart.Id);
        }

        private CartResponseModel MapCart(Cart cart)
        {
            var response =
                new CartResponseModel
                {
                    CartId = cart.Id
                };

            foreach (var item in cart.CartItems)
            {
                var image = item.Product.ProductImages.FirstOrDefault(x => x.IsPrimary);

                response.Items.Add(
                    new CartItemResponseModel
                    {
                        CartItemId = item.Id,
                        ProductId = item.ProductId,
                        ProductName = item.Product.Name,
                        ImageUrl = image?.ImageUrl ?? string.Empty,
                        Price = item.Product.Price,
                        Quantity = item.Quantity,
                        Total = item.Product.Price * item.Quantity
                    });
            }

            response.SubTotal = response.Items.Sum(x => x.Total);

            response.Shipping = response.SubTotal > 0 ? 50 : 0;

            response.Total = response.SubTotal + response.Shipping;

            return response;
        }
    }
}