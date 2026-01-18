using Shipments.Shared.Domain.Entities;
using Shipments.Shared.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipments.Shared.Contracts.Shipments.Responses
{
    public class ShipmentDetailsDto
    {
        public int Id { get; set; }
        public ShipmentStatus Status { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public string RecipientAddress { get; set; } = string.Empty;
        public string RecipientCity { get; set; } = string.Empty;
        public string RecipientPostalCode { get; set; } = string.Empty;
        public string RecipientPhone { get; set; } = string.Empty;
        
        public string ClientId { get; set; } = string.Empty;
        public string? CourierId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAtUtc { get; set; }

        public List<ShipmentEventDto> Events { get; set; } = new();

    }
}
