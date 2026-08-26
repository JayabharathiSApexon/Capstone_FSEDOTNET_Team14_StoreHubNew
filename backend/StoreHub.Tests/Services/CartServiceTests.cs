using AutoMapper;
using StoreHub.Application.Models.Cart;
using StoreHub.Application.Services;
using StoreHub.Domain.Entities;

namespace StoreHub.Tests.Services;

public class CartServiceTests
{
    [Fact]
    public async Task Given_ExistingCart_When_GetCart_Then_ReturnsTotalsWithShipping()
    {
        var product = new Product { Id = Guid.NewGuid(), Name = "Keyboard", Price = 25m, ProductImages = new List<ProductImage>() };
        var cart = new Cart { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), CartItems = new List<CartItem> { new() { Id = Guid.NewGuid(), ProductId = product.Id, Quantity = 2, Product = product } } };
        var repository = new TestCartRepository { Cart = cart };
        var service = new CartService(repository, new TestProductRepository(), new MapperConfiguration(_ => { }).CreateMapper());

        var result = await service.GetCartAsync(cart.UserId);

        Assert.Equal(50m, result.SubTotal);
        Assert.Equal(50m, result.Shipping);
        Assert.Equal(100m, result.Total);
    }

    [Fact]
    public async Task Given_NoCart_When_GetCart_Then_ReturnsEmptyCart()
    {
        var service = new CartService(new TestCartRepository(), new TestProductRepository(), new MapperConfiguration(_ => { }).CreateMapper());

        var result = await service.GetCartAsync(Guid.NewGuid());

        Assert.Empty(result.Items);
        Assert.Equal(0m, result.Total);
    }

    [Fact]
    public async Task Given_MissingProduct_When_AddToCart_Then_ThrowsException()
    {
        var service = new CartService(new TestCartRepository(), new TestProductRepository(), new MapperConfiguration(_ => { }).CreateMapper());

        await Assert.ThrowsAsync<Exception>(() => service.AddToCartAsync(Guid.NewGuid(), new AddToCartRequestModel { ProductId = Guid.NewGuid(), Quantity = 1 }));
    }

    [Fact]
    public async Task Given_NoCartAndExistingProduct_When_AddToCart_Then_CreatesCartAndAddsItem()
    {
        var userId = Guid.NewGuid();
        var product = new Product { Id = Guid.NewGuid(), Name = "Keyboard", Price = 40m, ProductImages = new List<ProductImage>() };
        var cartRepository = new TestCartRepository { ProductForAddedItem = product };
        var productRepository = new TestProductRepository();
        productRepository.Products.Add(product);
        var service = new CartService(cartRepository, productRepository, new AutoMapper.MapperConfiguration(_ => { }).CreateMapper());

        var result = await service.AddToCartAsync(userId, new AddToCartRequestModel { ProductId = product.Id, Quantity = 2 });

        Assert.Equal(1, cartRepository.CreateCartCalls);
        Assert.Equal(1, cartRepository.AddCartItemCalls);
        Assert.Equal(80m, result.SubTotal);
        Assert.Equal(2, result.Items[0].Quantity);
    }

    [Fact]
    public async Task Given_ExistingCartItem_When_AddToCart_Then_IncreasesQuantity()
    {
        var product = new Product { Id = Guid.NewGuid(), Name = "Mouse", Price = 15m, ProductImages = new List<ProductImage>() };
        var item = new CartItem { Id = Guid.NewGuid(), ProductId = product.Id, Quantity = 1, Product = product };
        var cart = new Cart { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), CartItems = new List<CartItem> { item } };
        var repository = new TestCartRepository { Cart = cart, CartItem = item };
        var productRepository = new TestProductRepository();
        productRepository.Products.Add(product);
        var service = new CartService(repository, productRepository, new AutoMapper.MapperConfiguration(_ => { }).CreateMapper());

        await service.AddToCartAsync(cart.UserId, new AddToCartRequestModel { ProductId = product.Id, Quantity = 3 });

        Assert.Equal(4, item.Quantity);
        Assert.Equal(1, repository.UpdateCartItemCalls);
    }

    [Fact]
    public async Task Given_MissingCart_When_UpdateCartItem_Then_ThrowsCartNotFound()
    {
        var service = new CartService(new TestCartRepository(), new TestProductRepository(), new AutoMapper.MapperConfiguration(_ => { }).CreateMapper());

        var exception = await Assert.ThrowsAsync<Exception>(() => service.UpdateCartItemAsync(Guid.NewGuid(), new UpdateCartRequestModel { CartItemId = Guid.NewGuid(), Quantity = 1 }));

        Assert.Equal("Cart not found.", exception.Message);
    }

    [Fact]
    public async Task Given_MissingCartItem_When_UpdateCartItem_Then_ThrowsCartItemNotFound()
    {
        var cart = new Cart { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), CartItems = new List<CartItem>() };
        var service = new CartService(new TestCartRepository { Cart = cart }, new TestProductRepository(), new AutoMapper.MapperConfiguration(_ => { }).CreateMapper());

        var exception = await Assert.ThrowsAsync<Exception>(() => service.UpdateCartItemAsync(cart.UserId, new UpdateCartRequestModel { CartItemId = Guid.NewGuid(), Quantity = 1 }));

        Assert.Equal("Cart item not found.", exception.Message);
    }

    [Fact]
    public async Task Given_ExistingCartItem_When_UpdateCartItem_Then_UpdatesQuantityAndTotals()
    {
        var product = new Product { Id = Guid.NewGuid(), Name = "Mouse", Price = 30m, ProductImages = new List<ProductImage>() };
        var cart = new Cart
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            CartItems = new List<CartItem>
            {
                new() { Id = Guid.NewGuid(), ProductId = product.Id, Quantity = 1, Product = product }
            }
        };
        var repository = new TestCartRepository { Cart = cart };
        var service = new CartService(repository, new TestProductRepository(), new MapperConfiguration(_ => { }).CreateMapper());

        var item = cart.CartItems.First();
        var result = await service.UpdateCartItemAsync(cart.UserId, new UpdateCartRequestModel { CartItemId = item.Id, Quantity = 4 });

        Assert.Equal(4, item.Quantity);
        Assert.Equal(170m, result.Total);
        Assert.Equal(50m, result.Shipping);
    }

    [Fact]
    public async Task Given_ExistingCart_When_RemoveCartItem_Then_DeletesSelectedItem()
    {
        var product = new Product { Id = Guid.NewGuid(), Name = "Monitor", Price = 100m, ProductImages = new List<ProductImage>() };
        var cart = new Cart
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            CartItems = new List<CartItem>
            {
                new() { Id = Guid.NewGuid(), ProductId = product.Id, Quantity = 1, Product = product }
            }
        };
        var repository = new TestCartRepository { Cart = cart };
        var service = new CartService(repository, new TestProductRepository(), new MapperConfiguration(_ => { }).CreateMapper());
        var itemId = cart.CartItems.First().Id;

        await service.RemoveCartItemAsync(cart.UserId, itemId);

        Assert.Equal(itemId, repository.DeletedItemId);
    }

    [Fact]
    public async Task Given_MissingCart_When_RemoveCartItem_Then_ThrowsCartNotFound()
    {
        var service = new CartService(new TestCartRepository(), new TestProductRepository(), new AutoMapper.MapperConfiguration(_ => { }).CreateMapper());

        await Assert.ThrowsAsync<Exception>(() => service.RemoveCartItemAsync(Guid.NewGuid(), Guid.NewGuid()));
    }

    [Fact]
    public async Task Given_EmptyCart_When_ClearCart_Then_DoesNotCallRepository()
    {
        var repository = new TestCartRepository();
        var service = new CartService(repository, new TestProductRepository(), new AutoMapper.MapperConfiguration(_ => { }).CreateMapper());

        await service.ClearCartAsync(Guid.NewGuid());

        Assert.Null(repository.ClearedCartId);
    }

    [Fact]
    public async Task Given_ExistingCart_When_ClearCart_Then_ClearsCartAndReturnsEmptySummary()
    {
        var product = new Product { Id = Guid.NewGuid(), Name = "Headset", Price = 80m, ProductImages = new List<ProductImage>() };
        var cart = new Cart
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            CartItems = new List<CartItem>
            {
                new() { Id = Guid.NewGuid(), ProductId = product.Id, Quantity = 2, Product = product }
            }
        };
        var repository = new TestCartRepository { Cart = cart };
        var service = new CartService(repository, new TestProductRepository(), new MapperConfiguration(_ => { }).CreateMapper());

        await service.ClearCartAsync(cart.UserId);

        Assert.Equal(cart.Id, repository.ClearedCartId);
    }
}