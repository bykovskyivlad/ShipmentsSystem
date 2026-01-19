using Shipments.Shared.Domain.Enums;

namespace Shipments.Shared.Contracts.Shipments.Responses;

public class ShipmentListItemDto
{
    public int Id { get; set; }
    public ShipmentStatus Status { get; set; }

    public string RecipientName { get; set; } = string.Empty;
    public string RecipientCity { get; set; } = string.Empty;

    public string ClientId { get; set; } = string.Empty;
    public string? CourierId { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
}
