using Shipments.Shared.Domain.Enums;

namespace Shipments.Shared.Contracts.Couriers.Requests;

public class UpdateShipmentStatusRequest
{
    public ShipmentStatus NewStatus { get; set; }
    public string? Notes { get; set; }
}