using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipments.Shared.Domain.Enums
{
    public enum ShipmentStatus
    {
        /// <summary>
        /// Przesyłka została utworzona
        /// </summary>
        Created = 1,
        /// <summary>
        /// Przesyłka została odebrana przez kuriera
        /// </summary>
        PickedUp = 2,
        /// <summary>
        /// Przesyłka jest w doreczeniu
        /// </summary>
        OutForDelivery = 3,
        /// <summary>
        /// Przesyłka została dostarczona
        /// </summary>
        Delivered = 4,
        /// <summary>
        /// Nie udało się dostarczyć przesyłki
        /// </summary>
        DeliveryFailed = 5,
        /// <summary>
        /// Odwołano przesyłkę
        /// </summary>
        Canceled = 6

    }
}
