using StoreHub.Application.Services;
using StoreHub.Domain.Entities;

namespace StoreHub.Tests.Services;

public class TrackingServiceTests
{
    [Fact]
    public async Task Given_MissingOrder_When_GetTrackingDetails_Then_ReturnsNull()
    {
        var service = new TrackingService(new TestTrackingRepository());

        var result = await service.GetTrackingDetailsAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task Given_OrderWithoutOrderPlacedHistory_When_GetTrackingDetails_Then_AddsOrderPlacedFirst()
    {
        var order = new Order { Id = Guid.NewGuid(), Status = "Processing", OrderDate = DateTime.UtcNow, TotalAmount = 75m, TrackingHistory = new List<OrderTrackingHistory> { new() { Status = "Processing", StatusDate = DateTime.UtcNow.AddHours(1) } } };
        var service = new TrackingService(new TestTrackingRepository { Order = order });

        var result = await service.GetTrackingDetailsAsync(order.Id);

        Assert.NotNull(result);
        Assert.Equal("OrderPlaced", result!.TrackingHistory[0].Status);
        Assert.Equal("Processing", result.TrackingHistory[1].Status);
        Assert.Equal(order.OrderDate.AddDays(3), result.ExpectedDeliveryDate);
    }

    [Fact]
    public async Task Given_CancelledOrder_When_GetTrackingDetails_Then_PutsCancelledLast()
    {
        var order = new Order { Id = Guid.NewGuid(), Status = "Cancelled", OrderDate = DateTime.UtcNow, TrackingHistory = new List<OrderTrackingHistory> { new() { Status = "Cancelled", StatusDate = DateTime.UtcNow.AddHours(2) }, new() { Status = "Pending", StatusDate = DateTime.UtcNow.AddHours(1) } } };
        var service = new TrackingService(new TestTrackingRepository { Order = order });

        var result = await service.GetTrackingDetailsAsync(order.Id);

        Assert.Equal("Cancelled", result!.TrackingHistory.Last().Status);
    }

    [Fact]
    public async Task Given_OrderItemWithPrimaryImage_When_GetTrackingDetails_Then_MapsProductImage()
    {
        var product = new Product { Id = Guid.NewGuid(), Name = "Headphones", ProductImages = new List<ProductImage> { new() { ImageUrl = "/headphones.png", IsPrimary = true } } };
        var order = new Order { Id = Guid.NewGuid(), OrderDate = DateTime.UtcNow, OrderItems = new List<OrderItem> { new() { ProductId = product.Id, Quantity = 1, Product = product } } };
        var service = new TrackingService(new TestTrackingRepository { Order = order });

        var result = await service.GetTrackingDetailsAsync(order.Id);

        Assert.Equal("Headphones", result!.Products[0].ProductName);
        Assert.Equal("/headphones.png", result.Products[0].ImageUrl);
    }
}