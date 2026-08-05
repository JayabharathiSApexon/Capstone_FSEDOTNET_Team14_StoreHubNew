using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StoreHub.Domain.Entities;

namespace StoreHub.Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId);

        Task<Order?> GetOrderByIdAsync(Guid orderId);
    }
}
