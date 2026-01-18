using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipments.Shared.Domain.Enums;

namespace Shipments.Shared.Contracts.Shipments.Responses
{
    public class ShipmentEventDto
    {
        public int Id { get; set; }

        public ShipmentStatus? OldStatus { get; set; }
        public ShipmentStatus? NewStatus { get; set; }

        public string PerformedByUserId { get; set; } = string.Empty;
        public string PerformedByRole { get; set; } = string.Empty;
        
        public string? Notes { get; set; }
        public DateTimeOffset OccurredAtUtc { get; set; }
    }
}
