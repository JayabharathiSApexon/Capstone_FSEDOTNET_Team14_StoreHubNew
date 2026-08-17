using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StoreHub.Application.Models.Order;

namespace StoreHub.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<MyOrderResponseModel>> GetOrdersByUserIdAsync(Guid userId);

        Task<IEnumerable<MyOrderResponseModel>> GetAllOrdersAsync();

        Task<bool> UpdateOrderStatusAsync(Guid orderId, string status);

        Task<bool> CancelOrderAsync(Guid orderId, Guid userId);

        Task<MyOrderResponseModel> CreateOrderAsync(OrderCreateRequest request);
    }
}
