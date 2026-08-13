using System;
using System.Threading.Tasks;
using StoreHub.Application.Interfaces.Repositories;
using StoreHub.Application.Interfaces.Services;
using StoreHub.Application.Models.Order;
using StoreHub.Domain.Entities;

namespace StoreHub.Application.Services
{
    public class TrackingService : ITrackingService
    {
        private readonly ITrackingDetails _trackingDetails;

        public TrackingService(ITrackingDetails trackingDetails)
        {
            _trackingDetails = trackingDetails;
        }

        public async Task<TrackingResponseModel?> GetTrackingDetailsAsync(Guid orderId)
        {
            var order = await _trackingDetails.GetTrackingDetailsAsync(orderId);

            if (order == null)
            {
                return null;
            }

            var statusOrder = new[]
            {
                "OrderPlaced",
                "Pending",
                "Processing",
                "Shipped",
                "Delivered"
            };

            var trackingHistory = order.TrackingHistory
                .GroupBy(x => x.Status)
                .Select(group =>
                    group
                        .OrderByDescending(x => x.StatusDate)
                        .First())
                .ToList();

            var hasOrderPlaced = trackingHistory.Any(x =>
                string.Equals(x.Status, "OrderPlaced", StringComparison.OrdinalIgnoreCase));

            if (!hasOrderPlaced)
            {
                trackingHistory.Add(
                    new OrderTrackingHistory
                    {
                        Id = Guid.Empty,
                        OrderId = order.Id,
                        Status = "OrderPlaced",
                        StatusDate = order.OrderDate,
                        Remarks = "Order placed.",
                        CreatedDate = order.OrderDate
                    });
            }

            // Sort according to the expected order.
            trackingHistory = trackingHistory
                .OrderBy(x =>
                {
                    var index = Array.FindIndex(
                        statusOrder,
                        status => string.Equals(
                            status,
                            x.Status,
                            StringComparison.OrdinalIgnoreCase));

                    return index == -1
                        ? int.MaxValue
                        : index;
                })
                .ThenBy(x => x.StatusDate)
                .ToList();

            // If cancelled, add it as the final status.
            if (string.Equals(order.Status, "Cancelled", StringComparison.OrdinalIgnoreCase))
            {
                var cancelledHistory =
                    trackingHistory.FirstOrDefault(x =>
                        string.Equals(x.Status, "Cancelled", StringComparison.OrdinalIgnoreCase));

                if (cancelledHistory != null)
                {
                    trackingHistory.Remove(cancelledHistory);
                    trackingHistory.Add(cancelledHistory);
                }
            }

            return new TrackingResponseModel
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                ExpectedDeliveryDate = order.OrderDate.AddDays(3),
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Products = order.OrderItems
                    .Select(item => new TrackingProductModel
                    {
                        ProductId = item.ProductId,
                        ProductName = item.Product.Name,
                        Quantity = item.Quantity,
                        ImageUrl = item.Product.ProductImages
                            .Where(x => x.IsPrimary)
                            .OrderBy(x => x.DisplayOrder)
                            .Select(x => x.ImageUrl)
                            .FirstOrDefault()

                            ?? item.Product.ProductImages
                                .OrderBy(x => x.DisplayOrder)
                                .Select(x => x.ImageUrl)
                                .FirstOrDefault()

                            ?? string.Empty
                    })
                    .ToList(),

                TrackingHistory = trackingHistory
                    .Select(x => new TrackingStatusModel
                    {
                        Status = x.Status,
                        StatusDate = x.StatusDate,
                        Completed = true
                    })
                    .ToList()
            };
        }
    }
}
