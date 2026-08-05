using System;
using System.Threading.Tasks;
using StoreHub.Application.Interfaces.Repositories;
using StoreHub.Application.Interfaces.Services;
using StoreHub.Application.Models.Order;

namespace StoreHub.Application.Services
{
    public class TrackingService : ITrackingService
    {
        private readonly ITrackingDetails _trackingDetails;

        public TrackingService(ITrackingDetails trackingDetails)
        {
            _trackingDetails = trackingDetails;
        }

        public async Task<TrackingResponseModel?> GetTrackingService(Guid orderId)
        {
            return await _trackingDetails.GetTrackingDetails(orderId);
        }
    }
}
