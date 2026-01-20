using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipments.Shared.Contracts.Admin
{
    public class CourierListItemDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public int ActiveShipmentsCount { get; set; }
        public bool IsFree { get; set; }
    }

}
