using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipments.Shared.Domain.Enums;

namespace Shipments.Shared.Domain.Entities
{
    public class ShipmentEvent
    {
        public int Id { get; set; }
        public int ShipmentId { get; set; }
        public Shipment? Shipment { get; set; }
        public ShipmentStatus? OldStatus { get; set; }
        public ShipmentStatus? NewStatus { get; set; }
        [Required, MaxLength(70)]
        public string PerformedByUserId { get; set; } = string.Empty;
        [Required, MaxLength(30)]
        public string PerformedByRole { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Notes { get; set; }
        public DateTimeOffset OccurredAtUtc { get; set; } = DateTimeOffset.UtcNow;
    }
}
