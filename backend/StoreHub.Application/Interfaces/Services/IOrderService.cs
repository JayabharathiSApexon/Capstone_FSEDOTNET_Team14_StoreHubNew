using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StoreHub.Application.Models.Order;

namespace StoreHub.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<MyOrderResponseModel>> GetOrdersByUserIdAsync(Guid userId);

        Task<MyOrderResponseModel> CreateOrderAsync(OrderCreateRequest request);
    }
}
