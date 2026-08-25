using StoreHub.Application.Services;
using StoreHub.Domain.Entities;

namespace StoreHub.Tests.Services;

public class InventoryServiceTests
{
    [Fact]
    public async Task Given_SufficientStock_When_ReserveStock_Then_DecreasesStock()
    {
        var product = new Product { Id = Guid.NewGuid(), StockQuantity = 10 };
        var repository = new TestProductRepository();
        repository.Products.Add(product);
        var service = new InventoryService(repository);

        var result = await service.ReserveStockAsync(product.Id, 3);

        Assert.True(result);
        Assert.Equal(7, product.StockQuantity);
    }

    [Fact]
    public async Task Given_InsufficientStock_When_ReserveStock_Then_ReturnsFalseWithoutChangingStock()
    {
        var product = new Product { Id = Guid.NewGuid(), StockQuantity = 2 };
        var repository = new TestProductRepository();
        repository.Products.Add(product);
        var service = new InventoryService(repository);

        var result = await service.ReserveStockAsync(product.Id, 3);

        Assert.False(result);
        Assert.Equal(2, product.StockQuantity);
    }

    [Fact]
    public async Task Given_NonPositiveQuantity_When_ReserveStock_Then_ThrowsArgumentException()
    {
        var service = new InventoryService(new TestProductRepository());

        await Assert.ThrowsAsync<ArgumentException>(() => service.ReserveStockAsync(Guid.NewGuid(), 0));
    }

    [Fact]
    public async Task Given_OrderItems_When_RestoreStock_Then_ReleasesEachItem()
    {
        var product = new Product { Id = Guid.NewGuid(), StockQuantity = 4 };
        var repository = new TestProductRepository();
        repository.Products.Add(product);
        var service = new InventoryService(repository);

        await service.RestoreStockAsync(new[] { new OrderItem { ProductId = product.Id, Quantity = 2 } });

        Assert.Equal(6, product.StockQuantity);
    }
}