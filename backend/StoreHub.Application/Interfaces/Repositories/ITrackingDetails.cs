using System;
using System.Threading.Tasks;
using StoreHub.Application.Models.Order;
using StoreHub.Domain.Entities;

namespace StoreHub.Application.Interfaces.Repositories
{
    public interface ITrackingDetails
    {
        Task<Order?> GetTrackingDetailsAsync(Guid orderId);
    }
}
