using Microsoft.EntityFrameworkCore;
using Shipments.Api.Data;
using Shipments.Shared.Auth;
using Shipments.Shared.Contracts.Couriers.Requests;
using Shipments.Shared.Contracts.Shipments.Requests;
using Shipments.Shared.Contracts.Shipments.Responses;
using Shipments.Shared.Domain.Entities;
using Shipments.Shared.Domain.Enums;
using Shipments.Shared.Domain.Rules;

namespace Shipments.Api.Services;

public class ShipmentService : IShipmentService
{
    private readonly ShipmentsDbContext _db;

    public ShipmentService(ShipmentsDbContext db)
    {
        _db = db;
    }

    public async Task<ShipmentDetailsDto> CreateAsync(CreateShipmentRequest request, string userId)
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

        return MapDetails(shipment);
    }

    public async Task<ShipmentDetailsDto> UpdateStatusAsync(
        int shipmentId,
        UpdateShipmentStatusRequest request,
        string performedByUserId,
        string performedByRole)
    {
        var shipment = await _db.Shipments
            .Include(s => s.Events)
            .FirstOrDefaultAsync(s => s.Id == shipmentId);

        if (shipment is null)
            throw new KeyNotFoundException("Shipment not found");

        if (performedByRole == Roles.Courier && shipment.CourierId != performedByUserId)
            throw new InvalidOperationException("Courier has no access to this shipment");

        var oldStatus = shipment.Status;
        var newStatus = request.NewStatus;

        if (!ShipmentStatusTransitions.CanTransition(oldStatus, newStatus))
        {
            var allowed = ShipmentStatusTransitions.GetAllowedTransitions(oldStatus);
            throw new InvalidOperationException(
                $"Invalid transition: {oldStatus} -> {newStatus}. Allowed: {string.Join(", ", allowed)}");
        }

        shipment.Status = newStatus;
        shipment.UpdatedAtUtc = DateTimeOffset.UtcNow;

        var ev = new ShipmentEvent
        {
            ShipmentId = shipment.Id,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            PerformedByUserId = performedByUserId,
            PerformedByRole = performedByRole,
            Notes = request.Notes,
            OccurredAtUtc = DateTimeOffset.UtcNow
        };

        shipment.Events.Add(ev);

        await _db.SaveChangesAsync();

        return MapDetails(shipment);
    }

    public async Task<List<ShipmentListItemDto>> GetForClientAsync(string clientId)
        => await _db.Shipments.AsNoTracking()
            .Where(s => s.ClientId == clientId)
            .OrderByDescending(s => s.CreatedAtUtc)
            .Select(MapListItemExpr)
            .ToListAsync();

    public async Task<List<ShipmentListItemDto>> GetForCourierAsync(string courierId)
        => await _db.Shipments.AsNoTracking()
            .Where(s => s.CourierId == courierId)
            .OrderByDescending(s => s.UpdatedAtUtc)
            .Select(MapListItemExpr)
            .ToListAsync();

    public async Task<List<ShipmentListItemDto>> GetAllAsync()
        => await _db.Shipments.AsNoTracking()
            .OrderByDescending(s => s.CreatedAtUtc)
            .Select(MapListItemExpr)
            .ToListAsync();
    private static ShipmentDetailsDto MapDetails(Shipment shipment) => new()
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
        Events = shipment.Events
            .OrderBy(e => e.OccurredAtUtc)
            .Select(e => new ShipmentEventDto
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

    private static readonly System.Linq.Expressions.Expression<Func<Shipment, ShipmentListItemDto>> MapListItemExpr =
        s => new ShipmentListItemDto
        {
            Id = s.Id,
            Status = s.Status,
            RecipientName = s.RecipientName,
            RecipientCity = s.RecipientCity,
            ClientId = s.ClientId,
            CourierId = s.CourierId,
            CreatedAtUtc = s.CreatedAtUtc,
            UpdatedAtUtc = s.UpdatedAtUtc
        };
}