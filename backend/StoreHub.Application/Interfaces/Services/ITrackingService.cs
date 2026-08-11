using System;
using System.Threading.Tasks;
using StoreHub.Application.Models.Order;

namespace StoreHub.Application.Interfaces.Services
{
    public interface ITrackingService
    {
        Task<TrackingResponseModel?> GetTrackingService(Guid orderId);
    }
}
