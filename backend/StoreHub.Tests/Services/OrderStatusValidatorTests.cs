using StoreHub.Application.Services;
using StoreHub.Domain.Enums;

namespace StoreHub.Tests.Services;

public class OrderStatusValidatorTests
{
    [Fact]
    public void Given_PendingOrder_When_TransitioningToProcessing_Then_ReturnsTrue()
    {
        var validator = new OrderStatusValidator();

        var result = validator.IsValidTransition(OrderStatus.Pending, OrderStatus.Processing);

        Assert.True(result);
    }

    [Fact]
    public void Given_DeliveredOrder_When_TransitioningToAnotherStatus_Then_ReturnsFalse()
    {
        var validator = new OrderStatusValidator();

        var result = validator.IsValidTransition(OrderStatus.Delivered, OrderStatus.Cancelled);

        Assert.False(result);
    }

    [Fact]
    public void Given_ProcessingOrder_When_GettingValidNextStatuses_Then_ReturnsShippedAndCancelled()
    {
        var validator = new OrderStatusValidator();

        var result = validator.GetValidNextStatuses(OrderStatus.Processing).ToList();

        Assert.Equal(new[] { OrderStatus.Shipped, OrderStatus.Cancelled }, result.OrderBy(x => x));
    }
}