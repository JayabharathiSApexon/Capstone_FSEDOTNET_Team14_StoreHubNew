using System;

namespace StoreHub.Application.Models.Order
{
    public class TrackingStatusModel
    {
        public string Status { get; set; } = string.Empty;

        public DateTime StatusDate { get; set; }

        public bool Completed { get; set; }
    }
}
