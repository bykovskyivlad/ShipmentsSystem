using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipments.Shared.Contracts.Shipments.Requests
{
    public class CreateShipmentRequest
    {
        [Required, MaxLength(100)]
        public string RecipientName { get; set; } = string.Empty;
        [Required, MaxLength(200)]
        public string RecipientAddress { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string RecipientCity { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string RecipientPostalCode { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string RecipientPhone { get; set; } = string.Empty;
        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
