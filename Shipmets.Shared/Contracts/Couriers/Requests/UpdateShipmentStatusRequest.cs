using Shipments.Shared.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipments.Shared.Contracts.Couriers.Requests
{
    public class UpdateShipmentStatusRequest
    {
        [Required]
        public ShipmentStatus NewStatus { get; set; }
        [MaxLength(500)]
        public string? Note { get; set; }
    }
}
