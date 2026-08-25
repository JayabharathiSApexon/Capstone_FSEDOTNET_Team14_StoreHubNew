using AutoMapper;
using StoreHub.Application.Mappings;
using StoreHub.Application.Models.Order;
using StoreHub.Application.Services;
using StoreHub.Domain.Entities;
using StoreHub.Domain.Enums;

namespace StoreHub.Tests.Services;

public class OrderServiceTests
{
    private static IMapper CreateMapper() => new MapperConfiguration(x => x.AddProfile<MappingProfile>()).CreateMapper();

    [Fact]
    public async Task Given_ValidOrder_When_CreateOrder_Then_ReservesStockAndCreatesPendingOrder()
    {
        var repository = new TestOrderRepository();
        var inventory = new RecordingInventoryService();
        var service = new OrderService(repository, inventory, new OrderStatusValidator(), CreateMapper());
        var productId = Guid.NewGuid();

        var result = await service.CreateOrderAsync(new OrderCreateRequest { UserId = Guid.NewGuid(), ShippingAddress = "1 Main St", ZipCode = "12345", TotalAmount = 40m, Items = new List<OrderItemRequest> { new() { ProductId = productId, Quantity = 2, UnitPrice = 20m, TotalPrice = 40m } } });

        Assert.Equal(OrderStatus.Pending.ToString(), result.Status);
        Assert.Single(inventory.Reservations);
        Assert.Single(repository.Orders[0].TrackingHistory);
    }

    [Fact]
    public async Task Given_MissingShippingAddress_When_CreateOrder_Then_ThrowsArgumentException()
    {
        var service = new OrderService(new TestOrderRepository(), new RecordingInventoryService(), new OrderStatusValidator(), CreateMapper());

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateOrderAsync(new OrderCreateRequest { ZipCode = "12345", TotalAmount = 10m, Items = new List<OrderItemRequest> { new() { Quantity = 1 } } }));
    }

    [Fact]
    public async Task Given_PendingOrder_When_UpdateStatusToProcessing_Then_SavesTrackingHistory()
    {
        var order = new Order { Id = Guid.NewGuid(), Status = OrderStatus.Pending.ToString() };
        var repository = new TestOrderRepository();
        repository.Orders.Add(order);
        var service = new OrderService(repository, new RecordingInventoryService(), new OrderStatusValidator(), CreateMapper());

        var result = await service.UpdateOrderStatusAsync(order.Id, "processing");

        Assert.True(result);
        Assert.Equal(OrderStatus.Processing.ToString(), order.Status);
        Assert.Equal(OrderStatus.Processing.ToString(), repository.SavedHistory!.Status);
    }

    [Fact]
    public async Task Given_OrderOwnedByAnotherUser_When_CancelOrder_Then_ThrowsUnauthorizedException()
    {
        var order = new Order { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Status = OrderStatus.Pending.ToString() };
        var repository = new TestOrderRepository();
        repository.Orders.Add(order);
        var service = new OrderService(repository, new RecordingInventoryService(), new OrderStatusValidator(), CreateMapper());

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CancelOrderAsync(order.Id, Guid.NewGuid()));
    }
}