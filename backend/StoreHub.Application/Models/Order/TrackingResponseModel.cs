using System;
using System.Collections.Generic;

namespace StoreHub.Application.Models.Order
{
    public class TrackingResponseModel
    {
        public Guid OrderId { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime ExpectedDeliveryDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = string.Empty;

        public List<TrackingProductModel> Products { get; set; } = new List<TrackingProductModel>();

        public List<TrackingStatusModel> TrackingHistory { get; set; } = new List<TrackingStatusModel>();
    }
}
