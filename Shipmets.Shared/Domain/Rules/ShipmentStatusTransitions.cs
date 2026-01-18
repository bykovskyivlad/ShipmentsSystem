using Shipments.Shared.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipments.Shared.Domain.Rules
{
    public static class ShipmentStatusTransitions
    {
        private static readonly Dictionary<ShipmentStatus, HashSet<ShipmentStatus>> Allowed = new()
        {
            [ShipmentStatus.Created] = new HashSet<ShipmentStatus> { ShipmentStatus.PickedUp, ShipmentStatus.Canceled },
            [ShipmentStatus.PickedUp] = new HashSet<ShipmentStatus> { ShipmentStatus.OutForDelivery },
            [ShipmentStatus.OutForDelivery] = new HashSet<ShipmentStatus> { ShipmentStatus.Delivered, ShipmentStatus.DeliveryFailed },
            [ShipmentStatus.Delivered] = new HashSet<ShipmentStatus>(),
            [ShipmentStatus.Canceled] = new HashSet<ShipmentStatus>()
        };
        /// <summary>
        /// Sprawdza, czy przejście ze statusu źródłowego do docelowego jest dozwolone.
        /// </summary>
        public static bool CanTransition(ShipmentStatus from, ShipmentStatus to) => Allowed.TryGetValue(from, out var targets) && targets.Contains(to);
        /// <summary>
        /// Zwraca listę dozwolonych statusów docelowych dla danego statusu źródłowego.
        /// </summary>
        public static IReadOnlyCollection<ShipmentStatus> GetAllowedTransitions(ShipmentStatus from) => Allowed.TryGetValue(from, out var targets) ? targets.ToArray() : Array.Empty<ShipmentStatus>();

    }
}
