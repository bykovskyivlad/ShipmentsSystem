using Shipments.Shared.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipments.Shared.Contracts.Shipments.Responses
{
    public class ShipmentListItemDto
    {
        public int Id { get; set; }
        public ShipmentStatus Status { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public string RecipientCity { get; set; } = string.Empty;
        public string RecipientPostalCode { get; set; } = string.Empty;

        public DateTimeOffset CreatedAtUtc { get; set; }

        public string? CourierId { get; set; }
    }
}
