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
    public async Task Given_NullRequest_When_CreateOrder_Then_ThrowsArgumentNullException()
    {
        var service = new OrderService(new TestOrderRepository(), new RecordingInventoryService(), new OrderStatusValidator(), CreateMapper());

        await Assert.ThrowsAsync<ArgumentNullException>(() => service.CreateOrderAsync(null!));
    }

    [Fact]
    public async Task Given_MissingZipCode_When_CreateOrder_Then_ThrowsArgumentException()
    {
        var service = new OrderService(new TestOrderRepository(), new RecordingInventoryService(), new OrderStatusValidator(), CreateMapper());

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateOrderAsync(new OrderCreateRequest { ShippingAddress = "Main St", TotalAmount = 10m, Items = new List<OrderItemRequest> { new() { Quantity = 1 } } }));
    }

    [Fact]
    public async Task Given_EmptyItems_When_CreateOrder_Then_ThrowsArgumentException()
    {
        var service = new OrderService(new TestOrderRepository(), new RecordingInventoryService(), new OrderStatusValidator(), CreateMapper());

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateOrderAsync(new OrderCreateRequest { ShippingAddress = "Main St", ZipCode = "12345", TotalAmount = 10m, Items = new List<OrderItemRequest>() }));
    }

    [Fact]
    public async Task Given_NonPositiveTotal_When_CreateOrder_Then_ThrowsArgumentException()
    {
        var service = new OrderService(new TestOrderRepository(), new RecordingInventoryService(), new OrderStatusValidator(), CreateMapper());

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateOrderAsync(new OrderCreateRequest { ShippingAddress = "Main St", ZipCode = "12345", TotalAmount = 0m, Items = new List<OrderItemRequest> { new() { Quantity = 1 } } }));
    }

    [Fact]
    public async Task Given_InsufficientStock_When_CreateOrder_Then_ThrowsAndDoesNotCreateOrder()
    {
        var repository = new TestOrderRepository();
        var inventory = new RecordingInventoryService { ReservationResult = false };
        var service = new OrderService(repository, inventory, new OrderStatusValidator(), CreateMapper());

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateOrderAsync(new OrderCreateRequest { ShippingAddress = "Main St", ZipCode = "12345", TotalAmount = 10m, Items = new List<OrderItemRequest> { new() { Quantity = 1 } } }));

        Assert.Empty(repository.Orders);
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
    public async Task Given_MissingOrder_When_UpdateStatus_Then_ReturnsFalse()
    {
        var service = new OrderService(new TestOrderRepository(), new RecordingInventoryService(), new OrderStatusValidator(), CreateMapper());

        Assert.False(await service.UpdateOrderStatusAsync(Guid.NewGuid(), "Processing"));
    }

    [Fact]
    public async Task Given_BlankStatus_When_UpdateStatus_Then_ThrowsArgumentException()
    {
        var service = new OrderService(new TestOrderRepository(), new RecordingInventoryService(), new OrderStatusValidator(), CreateMapper());

        await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateOrderStatusAsync(Guid.NewGuid(), " "));
    }

    [Fact]
    public async Task Given_UnknownStatus_When_UpdateStatus_Then_ThrowsArgumentException()
    {
        var service = new OrderService(new TestOrderRepository(), new RecordingInventoryService(), new OrderStatusValidator(), CreateMapper());

        await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateOrderStatusAsync(Guid.NewGuid(), "Unknown"));
    }

    [Fact]
    public async Task Given_InvalidCurrentStatus_When_UpdateStatus_Then_ThrowsInvalidOperationException()
    {
        var order = new Order { Id = Guid.NewGuid(), Status = "Unknown" };
        var repository = new TestOrderRepository();
        repository.Orders.Add(order);
        var service = new OrderService(repository, new RecordingInventoryService(), new OrderStatusValidator(), CreateMapper());

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateOrderStatusAsync(order.Id, "Processing"));
    }

    [Fact]
    public async Task Given_InvalidTransition_When_UpdateStatus_Then_ThrowsInvalidOperationException()
    {
        var order = new Order { Id = Guid.NewGuid(), Status = OrderStatus.Pending.ToString() };
        var repository = new TestOrderRepository();
        repository.Orders.Add(order);
        var service = new OrderService(repository, new RecordingInventoryService(), new OrderStatusValidator(), CreateMapper());

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateOrderStatusAsync(order.Id, "Delivered"));
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

    [Fact]
    public async Task Given_MissingOrder_When_CancelOrder_Then_ReturnsFalse()
    {
        var service = new OrderService(new TestOrderRepository(), new RecordingInventoryService(), new OrderStatusValidator(), CreateMapper());

        Assert.False(await service.CancelOrderAsync(Guid.NewGuid(), Guid.NewGuid()));
    }

    [Fact]
    public async Task Given_AlreadyCancelledOrder_When_CancelOrder_Then_ThrowsException()
    {
        var userId = Guid.NewGuid();
        var order = new Order { Id = Guid.NewGuid(), UserId = userId, Status = OrderStatus.Cancelled.ToString() };
        var repository = new TestOrderRepository();
        repository.Orders.Add(order);
        var service = new OrderService(repository, new RecordingInventoryService(), new OrderStatusValidator(), CreateMapper());

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CancelOrderAsync(order.Id, userId));
    }

    [Fact]
    public async Task Given_ShippedOrder_When_CancelOrder_Then_ThrowsException()
    {
        var userId = Guid.NewGuid();
        var order = new Order { Id = Guid.NewGuid(), UserId = userId, Status = OrderStatus.Shipped.ToString() };
        var repository = new TestOrderRepository();
        repository.Orders.Add(order);
        var service = new OrderService(repository, new RecordingInventoryService(), new OrderStatusValidator(), CreateMapper());

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CancelOrderAsync(order.Id, userId));
    }

    [Fact]
    public async Task Given_InvalidOrderStatus_When_CancelOrder_Then_ThrowsException()
    {
        var userId = Guid.NewGuid();
        var order = new Order { Id = Guid.NewGuid(), UserId = userId, Status = "Unknown" };
        var repository = new TestOrderRepository();
        repository.Orders.Add(order);
        var service = new OrderService(repository, new RecordingInventoryService(), new OrderStatusValidator(), CreateMapper());

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CancelOrderAsync(order.Id, userId));
    }

    [Fact]
    public async Task Given_PendingOrder_When_CancelOrder_Then_SavesCancellationAndRestoresStock()
    {
        var userId = Guid.NewGuid();
        var order = new Order { Id = Guid.NewGuid(), UserId = userId, Status = OrderStatus.Pending.ToString(), OrderItems = new List<OrderItem>() };
        var repository = new TestOrderRepository();
        repository.Orders.Add(order);
        var inventory = new RecordingInventoryService();
        var service = new OrderService(repository, inventory, new OrderStatusValidator(), CreateMapper());

        Assert.True(await service.CancelOrderAsync(order.Id, userId));
        Assert.Equal(OrderStatus.Cancelled.ToString(), order.Status);
        Assert.Equal(OrderStatus.Cancelled.ToString(), repository.SavedHistory!.Status);
        Assert.NotNull(inventory.RestoredItems);
    }

    [Fact]
    public async Task Given_RestoreFailure_When_CancelOrder_Then_WrapsFailure()
    {
        var userId = Guid.NewGuid();
        var order = new Order { Id = Guid.NewGuid(), UserId = userId, Status = OrderStatus.Pending.ToString() };
        var repository = new TestOrderRepository();
        repository.Orders.Add(order);
        var service = new OrderService(repository, new RecordingInventoryService { ThrowOnRestore = true }, new OrderStatusValidator(), CreateMapper());

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CancelOrderAsync(order.Id, userId));

        Assert.Contains("stock restoration failed", exception.Message);
    }

    [Fact]
    public async Task Given_Orders_When_GetOrdersByUserAndGetAllOrders_Then_ReturnsMappedCollections()
    {
        var userId = Guid.NewGuid();
        var repository = new TestOrderRepository();
        repository.Orders.Add(new Order { Id = Guid.NewGuid(), UserId = userId, Status = OrderStatus.Pending.ToString() });
        var service = new OrderService(repository, new RecordingInventoryService(), new OrderStatusValidator(), CreateMapper());

        var userOrders = await service.GetOrdersByUserIdAsync(userId);
        var allOrders = await service.GetAllOrdersAsync();

        Assert.Single(userOrders);
        Assert.Single(allOrders);
    }
}