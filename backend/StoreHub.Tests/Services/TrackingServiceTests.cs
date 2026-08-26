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

    [Fact]
    public async Task Given_OrderItemWithoutPrimaryImage_When_GetTrackingDetails_Then_UsesFirstImage()
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Camera",
            ProductImages = new List<ProductImage>
            {
                new() { ImageUrl = "/second.png", DisplayOrder = 2 },
                new() { ImageUrl = "/first.png", DisplayOrder = 1 }
            }
        };
        var order = new Order { Id = Guid.NewGuid(), Status = "Pending", OrderDate = DateTime.UtcNow, OrderItems = new List<OrderItem> { new() { ProductId = product.Id, Product = product, Quantity = 1 } } };

        var result = await new TrackingService(new TestTrackingRepository { Order = order }).GetTrackingDetailsAsync(order.Id);

        Assert.Equal("/first.png", result!.Products[0].ImageUrl);
    }

    [Fact]
    public async Task Given_OrderItemWithoutImages_When_GetTrackingDetails_Then_UsesEmptyImageUrl()
    {
        var product = new Product { Id = Guid.NewGuid(), Name = "Cable", ProductImages = new List<ProductImage>() };
        var order = new Order { Id = Guid.NewGuid(), Status = "Pending", OrderDate = DateTime.UtcNow, OrderItems = new List<OrderItem> { new() { ProductId = product.Id, Product = product, Quantity = 1 } } };

        var result = await new TrackingService(new TestTrackingRepository { Order = order }).GetTrackingDetailsAsync(order.Id);

        Assert.Equal(string.Empty, result!.Products[0].ImageUrl);
    }

    [Fact]
    public async Task Given_DuplicateTrackingStatuses_When_GetTrackingDetails_Then_UsesLatestEntry()
    {
        var latest = DateTime.UtcNow.AddHours(2);
        var order = new Order
        {
            Id = Guid.NewGuid(), Status = "Processing", OrderDate = DateTime.UtcNow,
            TrackingHistory = new List<OrderTrackingHistory>
            {
                new() { Status = "Processing", StatusDate = DateTime.UtcNow.AddHours(1) },
                new() { Status = "Processing", StatusDate = latest }
            }
        };

        var result = await new TrackingService(new TestTrackingRepository { Order = order }).GetTrackingDetailsAsync(order.Id);

        Assert.Single(result!.TrackingHistory.Where(x => x.Status == "Processing"));
        Assert.Equal(latest, result.TrackingHistory.First(x => x.Status == "Processing").StatusDate);
    }

    [Fact]
    public async Task Given_EmptyTrackingHistory_When_GetTrackingDetails_Then_ReturnsOrderPlaced()
    {
        var order = new Order { Id = Guid.NewGuid(), Status = "Pending", OrderDate = DateTime.UtcNow, TrackingHistory = new List<OrderTrackingHistory>() };

        var result = await new TrackingService(new TestTrackingRepository { Order = order }).GetTrackingDetailsAsync(order.Id);

        Assert.Single(result!.TrackingHistory);
        Assert.Equal("OrderPlaced", result.TrackingHistory[0].Status);
    }

    [Fact]
    public async Task Given_OrderWithoutOrderPlacedStatus_When_GetTrackingDetails_Then_AddsInitialStatus()
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            Status = "Pending",
            OrderDate = DateTime.UtcNow,
            TrackingHistory = new List<OrderTrackingHistory>
            {
                new() { Status = "Pending", StatusDate = DateTime.UtcNow.AddMinutes(5) }
            }
        };
        var service = new TrackingService(new TestTrackingRepository { Order = order });

        var result = await service.GetTrackingDetailsAsync(order.Id);

        Assert.NotNull(result);
        Assert.Contains(result!.TrackingHistory, x => x.Status == "OrderPlaced");
    }
}