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
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
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

            // Restore stock for cancelled order items
            foreach (var orderItem in order.OrderItems)
            {
                var product = await _productRepository.GetProductByIdAsync(orderItem.ProductId);
                if (product != null)
                {
                    product.StockQuantity += orderItem.Quantity;
                    product.UpdatedDate = DateTime.UtcNow;
                    await _productRepository.UpdateProductAsync(product);
                }
            }

            return true;
        }

        public async Task<MyOrderResponseModel> CreateOrderAsync(OrderCreateRequest request)
        {
            if (request == null)
            {
                throw new Exception("Order request is required.");
            }

            if (string.IsNullOrWhiteSpace(request.ShippingAddress))
            {
                throw new Exception("Shipping address is required.");
            }

            if (string.IsNullOrWhiteSpace(request.ZipCode))
            {
                throw new Exception("ZipCode is required.");
            }

            if (request.Items == null || !request.Items.Any())
            {
                throw new Exception("Order must contain at least one item.");
            }

            if (request.TotalAmount <= 0)
            {
                throw new Exception("Total amount must be greater than zero.");
            }

            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                TotalAmount = request.TotalAmount,
                Status = "Pending",
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
                Status = "Pending",
                StatusDate = DateTime.UtcNow,
                Remarks = "Order placed successfully",
                CreatedDate = DateTime.UtcNow
            };
            order.TrackingHistory.Add(initialTracking);

            // Save to database
            await _orderRepository.AddOrderAsync(order);
            await _orderRepository.SaveChangesAsync();

            // Reduce stock for each product
            foreach (var item in request.Items)
            {
                var product = await _productRepository.GetProductByIdAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity -= item.Quantity;
                    if (product.StockQuantity < 0)
                    {
                        product.StockQuantity = 0;
                    }
                    product.UpdatedDate = DateTime.UtcNow;
                    await _productRepository.UpdateProductAsync(product);
                }
            }

            // Map and return response
            return _mapper.Map<MyOrderResponseModel>(order);
        }
    }
}
