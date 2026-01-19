using Shipments.Api.Data;
using Shipments.Api.Services;
using Shipments.Shared.Auth;
using Shipments.Shared.Contracts.Shipments.Requests;
using Shipments.Shared.Contracts.Shipments.Responses;
using Shipments.Shared.Domain.Entities;
using Shipments.Shared.Domain.Enums;

namespace Shipments.Api.Services;

public class ShipmentService : IShipmentService
{
    private readonly ShipmentsDbContext _db;

    public ShipmentService(ShipmentsDbContext db)
    {
        _db = db;
    }

    public async Task<ShipmentDetailsDto> CreateAsync(
        CreateShipmentRequest request,
        string userId)
    {
        
        var shipment = new Shipment
        {
            ClientId = userId,
            RecipientName = request.RecipientName,
            RecipientAddress = request.RecipientAddress,
            RecipientCity = request.RecipientCity,
            RecipientPostalCode = request.RecipientPostalCode,
            RecipientPhone = request.RecipientPhone,
            Status = ShipmentStatus.Created,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };

      
        var createdEvent = new ShipmentEvent
        {
            Shipment = shipment,
            OldStatus = null,
            NewStatus = ShipmentStatus.Created,
            PerformedByUserId = userId,
            PerformedByRole = Roles.Client,
            Notes = request.Notes,
            OccurredAtUtc = DateTimeOffset.UtcNow
        };

        shipment.Events.Add(createdEvent);

       
        _db.Shipments.Add(shipment);
        await _db.SaveChangesAsync();


        return new ShipmentDetailsDto
        {
            Id = shipment.Id,
            Status = shipment.Status,
            RecipientName = shipment.RecipientName,
            RecipientAddress = shipment.RecipientAddress,
            RecipientCity = shipment.RecipientCity,
            RecipientPostalCode = shipment.RecipientPostalCode,
            RecipientPhone = shipment.RecipientPhone,

            
            ClientId = shipment.ClientId,
            CourierId = shipment.CourierId,
            CreatedAt = shipment.CreatedAtUtc,
            UpdatedAtUtc = shipment.UpdatedAtUtc,

            Events = shipment.Events.Select(e => new ShipmentEventDto
            {
                Id = e.Id,
                OldStatus = e.OldStatus,
                NewStatus = e.NewStatus,
                PerformedByUserId = e.PerformedByUserId,
                PerformedByRole = e.PerformedByRole,
                Notes = e.Notes,
                OccurredAtUtc = e.OccurredAtUtc  
            }).ToList()
        };

    }
}
