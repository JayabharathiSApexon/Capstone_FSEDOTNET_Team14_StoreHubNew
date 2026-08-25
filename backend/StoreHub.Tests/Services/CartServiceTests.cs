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
}