using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using StoreHub.Application.Interfaces.Repositories;
using StoreHub.Application.Interfaces.Services;
using StoreHub.Application.Models.Order;
using StoreHub.Domain.Entities;
using StoreHub.Domain.Enums;

namespace StoreHub.Application.Services
{
    /// <summary>
    /// Service for managing order operations.
    /// Follows Single Responsibility Principle by delegating inventory concerns to IInventoryService
    /// and status validation to IOrderStatusValidator.
    /// Follows Dependency Inversion Principle by depending on abstractions, not concrete implementations.
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IInventoryService _inventoryService;
        private readonly IOrderStatusValidator _statusValidator;
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepository,
            IInventoryService inventoryService,
            IOrderStatusValidator statusValidator,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _inventoryService = inventoryService;
            _statusValidator = statusValidator;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MyOrderResponseModel>> GetOrdersByUserIdAsync(Guid userId)
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<MyOrderResponseModel>>(orders);
        }

        public async Task<IEnumerable<MyOrderResponseModel>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();

            return _mapper.Map<IEnumerable<MyOrderResponseModel>>(orders);
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid orderId, string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                throw new ArgumentException("Order status is required.", nameof(status));
            }

            if (!Enum.TryParse<OrderStatus>(status.Trim(), ignoreCase: true, out var newStatus))
            {
                var validStatuses = string.Join(", ", Enum.GetNames(typeof(OrderStatus)));
                throw new ArgumentException(
                    $"Invalid order status. Valid statuses are: {validStatuses}", 
                    nameof(status));
            }

            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return false;
            }

            // Parse current status for validation
            if (!Enum.TryParse<OrderStatus>(order.Status, ignoreCase: true, out var currentStatus))
            {
                throw new InvalidOperationException($"Order has invalid status: {order.Status}");
            }

            // Validate status transition
            if (!_statusValidator.IsValidTransition(currentStatus, newStatus))
            {
                var validNextStatuses = string.Join(", ", _statusValidator.GetValidNextStatuses(currentStatus));
                throw new InvalidOperationException(
                    $"Cannot transition from '{order.Status}' to '{newStatus}'. " +
                    $"Valid next statuses are: {validNextStatuses}");
            }

            var currentDate = DateTime.UtcNow;
            order.Status = newStatus.ToString();
            order.UpdatedDate = currentDate;

            var trackingHistory = new OrderTrackingHistory
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Status = newStatus.ToString(),
                StatusDate = currentDate,
                Remarks = $"Order status updated to {newStatus}.",
                CreatedDate = currentDate
            };

            await _orderRepository.SaveOrderStatusChangeAsync(order, trackingHistory);
            return true;
        }

        public async Task<bool> CancelOrderAsync(Guid orderId, Guid userId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return false;
            }

            if (order.UserId != userId)
            {
                throw new InvalidOperationException("You are not authorized to cancel this order.");
            }

            if (string.Equals(order.Status, OrderStatus.Cancelled.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Order is already cancelled.");
            }

            // Parse current status for validation
            if (!Enum.TryParse<OrderStatus>(order.Status, ignoreCase: true, out var currentStatus))
            {
                throw new InvalidOperationException($"Order has invalid status: {order.Status}");
            }

            // Validate that cancellation is allowed from current status
            if (!_statusValidator.IsValidTransition(currentStatus, OrderStatus.Cancelled))
            {
                throw new InvalidOperationException("This order cannot be cancelled.");
            }

            var currentDate = DateTime.UtcNow;
            order.Status = OrderStatus.Cancelled.ToString();
            order.UpdatedDate = currentDate;

            var trackingHistory = new OrderTrackingHistory
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Status = OrderStatus.Cancelled.ToString(),
                StatusDate = currentDate,
                Remarks = "Order cancelled by customer.",
                CreatedDate = currentDate
            };

            await _orderRepository.SaveOrderStatusChangeAsync(order, trackingHistory);

            // Restore stock for cancelled order items using IInventoryService (SRP and DIP)
            try
            {
                await _inventoryService.RestoreStockAsync(order.OrderItems);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException(
                    "Order was cancelled but stock restoration failed. Please contact support.", ex);
            }

            return true;
        }

        public async Task<MyOrderResponseModel> CreateOrderAsync(OrderCreateRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Order request is required.");
            }

            if (string.IsNullOrWhiteSpace(request.ShippingAddress))
            {
                throw new ArgumentException("Shipping address is required.", nameof(request.ShippingAddress));
            }

            if (string.IsNullOrWhiteSpace(request.ZipCode))
            {
                throw new ArgumentException("ZipCode is required.", nameof(request.ZipCode));
            }

            if (request.Items == null || !request.Items.Any())
            {
                throw new ArgumentException("Order must contain at least one item.", nameof(request.Items));
            }

            if (request.TotalAmount <= 0)
            {
                throw new ArgumentException("Total amount must be greater than zero.", nameof(request.TotalAmount));
            }

            // Validate and reserve stock for all items before creating order
            foreach (var item in request.Items)
            {
                var reserved = await _inventoryService.ReserveStockAsync(item.ProductId, item.Quantity);
                if (!reserved)
                {
                    throw new InvalidOperationException(
                        $"Insufficient stock for product ID {item.ProductId}. Cannot complete order.");
                }
            }

            // Create order with reserved items
            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                TotalAmount = request.TotalAmount,
                Status = OrderStatus.Pending.ToString(),
                ShippingAddress = request.ShippingAddress,
                City = request.City,
                State = request.State ?? string.Empty,
                ZipCode = request.ZipCode,
                PaymentMethod = request.PaymentMethod,
                OrderDate = DateTime.UtcNow,
                OrderItems = new List<OrderItem>(),
                TrackingHistory = new List<OrderTrackingHistory>()
            };

            // Add order items
            foreach (var item in request.Items)
            {
                order.OrderItems.Add(new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice,
                    CreatedDate = DateTime.UtcNow
                });
            }

            // Add initial tracking history
            var initialTracking = new OrderTrackingHistory
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Status = OrderStatus.Pending.ToString(),
                StatusDate = DateTime.UtcNow,
                Remarks = "Order placed successfully",
                CreatedDate = DateTime.UtcNow
            };
            order.TrackingHistory.Add(initialTracking);

            // Save to database
            await _orderRepository.AddOrderAsync(order);
            await _orderRepository.SaveChangesAsync();

            // Map and return response
            return _mapper.Map<MyOrderResponseModel>(order);
        }
    }
}
