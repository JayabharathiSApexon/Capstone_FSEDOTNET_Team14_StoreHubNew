using System.ComponentModel.DataAnnotations.Schema;

namespace StoreHub.Domain.Entities
{
    [Table("OrderTrackingHistory")]
    public class OrderTrackingHistory
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime StatusDate { get; set; }

        public string Remarks { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public Order Order { get; set; } = null!;
    }
}
