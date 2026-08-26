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

    [Theory]
    [InlineData(OrderStatus.Pending, OrderStatus.Cancelled)]
    [InlineData(OrderStatus.Processing, OrderStatus.Shipped)]
    [InlineData(OrderStatus.Processing, OrderStatus.Cancelled)]
    [InlineData(OrderStatus.Shipped, OrderStatus.Delivered)]
    public void Given_AllowedTransition_When_Validating_Then_ReturnsTrue(OrderStatus current, OrderStatus next)
    {
        Assert.True(new OrderStatusValidator().IsValidTransition(current, next));
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

    [Fact]
    public void Given_SameStatus_When_Transitioning_Then_ReturnsFalse()
    {
        var validator = new OrderStatusValidator();

        var result = validator.IsValidTransition(OrderStatus.Pending, OrderStatus.Pending);

        Assert.False(result);
    }

    [Fact]
    public void Given_InvalidCurrentStatus_When_GettingValidNextStatuses_Then_ReturnsEmpty()
    {
        var validator = new OrderStatusValidator();

        var result = validator.GetValidNextStatuses((OrderStatus)999).ToList();

        Assert.Empty(result);
    }

    [Theory]
    [InlineData(OrderStatus.Delivered)]
    [InlineData(OrderStatus.Cancelled)]
    public void Given_TerminalStatus_When_GettingValidNextStatuses_Then_ReturnsEmpty(OrderStatus status)
    {
        Assert.Empty(new OrderStatusValidator().GetValidNextStatuses(status));
    }

    [Fact]
    public void Given_InvalidCurrentStatus_When_ValidatingTransition_Then_ReturnsFalse()
    {
        Assert.False(new OrderStatusValidator().IsValidTransition((OrderStatus)999, OrderStatus.Pending));
    }

    [Fact]
    public void Given_ValidCurrentAndInvalidTarget_When_ValidatingTransition_Then_ReturnsFalse()
    {
        Assert.False(new OrderStatusValidator().IsValidTransition(OrderStatus.Pending, (OrderStatus)999));
    }
}