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

        public async Task<MyOrderResponseModel> CreateOrderAsync(OrderCreateRequest request)
        {
            // Create new order
            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                TotalAmount = request.TotalAmount,
                Status = "Pending",
                ShippingAddress = request.ShippingAddress,
                City = request.City,
                State = string.Empty,
                ZipCode = request.ZipCode,
                PaymentMethod = request.PaymentMethod,
                OrderDate = DateTime.UtcNow,
                UpdatedDate = null,
                OrderItems = new List<OrderItem>(),
                TrackingHistory = new List<OrderTrackingHistory>()
            };

            // Add order items
            foreach (var item in request.Items)
            {
                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = null
                };

                order.OrderItems.Add(orderItem);
            }

            // Create initial tracking history
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

            // Return mapped response
            return _mapper.Map<MyOrderResponseModel>(order);
        }
    }
}
