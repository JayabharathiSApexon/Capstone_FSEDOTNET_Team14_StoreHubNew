using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using StoreHub.Application.Interfaces.Repositories;
using StoreHub.Application.Interfaces.Services;
using StoreHub.Application.Models.Order;
using StoreHub.Domain.Entities;

namespace StoreHub.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
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
                throw new Exception("Order status is required.");
            }

            var newStatus = status.Trim();

            var allowedStatuses = new[]
            {
                "Pending",
                "Processing",
                "Shipped",
                "Delivered",
                "Cancelled"
            };

            if (!allowedStatuses.Contains(newStatus, StringComparer.OrdinalIgnoreCase))
            {
                throw new Exception("Invalid order status.");
            }

            var order = await _orderRepository.GetOrderByIdAsync(orderId);

            if (order == null)
            {
                return false;
            }

            if (string.Equals(order.Status, newStatus, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception($"Order status is already '{order.Status}'.");
            }

            var currentDate = DateTime.UtcNow;

            order.Status = newStatus;
            order.UpdatedDate = currentDate;

            var trackingHistory = new OrderTrackingHistory
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Status = newStatus,
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
                throw new Exception("You are not authorized to cancel this order.");
            }

            if (string.Equals(order.Status, "Cancelled", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Order is already cancelled.");
            }

            if (string.Equals(order.Status, "Shipped", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(order.Status, "Delivered", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("This order cannot be cancelled.");
            }

            var currentDate = DateTime.UtcNow;

            order.Status = "Cancelled";
            order.UpdatedDate = currentDate;

            var trackingHistory = new OrderTrackingHistory
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Status = "Cancelled",
                StatusDate = currentDate,
                Remarks = "Order cancelled by customer.",
                CreatedDate = currentDate
            };

            await _orderRepository.SaveOrderStatusChangeAsync(order, trackingHistory);

            return true;
        }
    }
}
