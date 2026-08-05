using System;
using System.Threading.Tasks;
using StoreHub.Application.Models.Order;

namespace StoreHub.Application.Interfaces.Repositories
{
    public interface ITrackingDetails
    {
        Task<TrackingResponseModel?> GetTrackingDetails(Guid orderId);
    }
}
