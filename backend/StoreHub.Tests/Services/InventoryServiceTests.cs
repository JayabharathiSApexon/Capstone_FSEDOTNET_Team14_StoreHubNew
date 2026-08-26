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
    public async Task Given_MissingProduct_When_ReserveStock_Then_ThrowsArgumentException()
    {
        var service = new InventoryService(new TestProductRepository());

        await Assert.ThrowsAsync<ArgumentException>(() => service.ReserveStockAsync(Guid.NewGuid(), 1));
    }

    [Fact]
    public async Task Given_ExactStockQuantity_When_ReserveStock_Then_LeavesZeroStock()
    {
        var product = new Product { Id = Guid.NewGuid(), StockQuantity = 5 };
        var repository = new TestProductRepository();
        repository.Products.Add(product);

        var result = await new InventoryService(repository).ReserveStockAsync(product.Id, 5);

        Assert.True(result);
        Assert.Equal(0, product.StockQuantity);
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

    [Fact]
    public async Task Given_ProductExists_When_ReleaseStock_Then_IncreasesStock()
    {
        var product = new Product { Id = Guid.NewGuid(), StockQuantity = 4 };
        var repository = new TestProductRepository();
        repository.Products.Add(product);
        var service = new InventoryService(repository);

        await service.ReleaseStockAsync(product.Id, 2);

        Assert.Equal(6, product.StockQuantity);
    }

    [Fact]
    public async Task Given_NonPositiveQuantity_When_ReleaseStock_Then_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => new InventoryService(new TestProductRepository()).ReleaseStockAsync(Guid.NewGuid(), 0));
    }

    [Fact]
    public async Task Given_MissingProduct_When_ReleaseStock_Then_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => new InventoryService(new TestProductRepository()).ReleaseStockAsync(Guid.NewGuid(), 1));
    }

    [Fact]
    public async Task Given_ExistingProduct_When_GetStockQuantity_Then_ReturnsCurrentStock()
    {
        var product = new Product { Id = Guid.NewGuid(), StockQuantity = 12 };
        var repository = new TestProductRepository();
        repository.Products.Add(product);

        var result = await new InventoryService(repository).GetStockQuantityAsync(product.Id);

        Assert.Equal(12, result);
    }

    [Fact]
    public async Task Given_MissingProduct_When_GetStockQuantity_Then_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => new InventoryService(new TestProductRepository()).GetStockQuantityAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task Given_EmptyOrderItems_When_RestoreStock_Then_DoesNothing()
    {
        var repository = new TestProductRepository();
        var service = new InventoryService(repository);

        await service.RestoreStockAsync(Array.Empty<OrderItem>());

        Assert.Equal(0, repository.UpdateCalls);
    }

    [Fact]
    public async Task Given_NullOrderItems_When_RestoreStock_Then_DoesNothing()
    {
        var repository = new TestProductRepository();

        await new InventoryService(repository).RestoreStockAsync(null!);

        Assert.Equal(0, repository.UpdateCalls);
    }

    [Fact]
    public async Task Given_MissingProductInOrderItems_When_RestoreStock_Then_WrapsFailure()
    {
        var productId = Guid.NewGuid();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => new InventoryService(new TestProductRepository()).RestoreStockAsync(new[] { new OrderItem { ProductId = productId, Quantity = 1 } }));

        Assert.Contains(productId.ToString(), exception.Message);
    }
}