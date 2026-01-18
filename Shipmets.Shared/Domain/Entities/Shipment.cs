using Shipments.Shared.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;



namespace Shipments.Shared.Domain.Entities
{
    public class Shipment
    {
       
        public int Id { get; set; }
        [Required]
        [MaxLength(70)]
        public string ClientId { get; set; }
        [MaxLength(70)]
        public string? CourierId { get; set; }
        [Required,MaxLength(100)]
        public string RecipientName { get; set; } = string.Empty;
        [Required, MaxLength(200)]
        public string RecipientAddress { get; set; } = string.Empty;
        [Required, MaxLength(50)]
        public string RecipientCity { get; set; } = string.Empty;
        [Required, MaxLength(20)]
        public string RecipientPostalCode { get; set; } = string.Empty;
        [Required, MaxLength(20)]
        public string RecipientPhone { get; set; } = string.Empty;
        public ShipmentStatus Status { get; set; } = ShipmentStatus.Created;
        public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAtUtc { get;set; }

        public List<ShipmentEvent> Events { get; set; } = new();

    }
}
