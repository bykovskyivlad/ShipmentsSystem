using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shipments.Api.Data;
using Shipments.Api.Models;
using Shipments.Shared.Auth;
using Shipments.Shared.Contracts.Admin;
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
    private readonly UserManager<AppUser> _userManager;

    public ShipmentService(ShipmentsDbContext db, UserManager<AppUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    

    public async Task<ShipmentDetailsDto> CreateAsync(CreateShipmentRequest request, string userId)
    {
        var now = DateTimeOffset.UtcNow;

        var shipment = new Shipment
        {
            ClientId = userId,
            RecipientName = request.RecipientName,
            RecipientAddress = request.RecipientAddress,
            RecipientCity = request.RecipientCity,
            RecipientPostalCode = request.RecipientPostalCode,
            RecipientPhone = request.RecipientPhone,
            Status = ShipmentStatus.Created,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        var createdEvent = new ShipmentEvent
        {
            Shipment = shipment,
            OldStatus = null,
            NewStatus = ShipmentStatus.Created,
            PerformedByUserId = userId,
            PerformedByRole = Roles.Client,
            Notes = request.Notes,
            OccurredAtUtc = now
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

    

    public async Task<ShipmentDetailsDto> AssignCourierAsync(
        int shipmentId,
        string courierId,
        string performedByUserId,
        string performedByRole)
    {
        var shipment = await _db.Shipments
            .Include(s => s.Events)
            .FirstOrDefaultAsync(s => s.Id == shipmentId);

        if (shipment is null)
            throw new KeyNotFoundException("Shipment not found");

        
        if (shipment.Status == ShipmentStatus.Delivered)
            throw new InvalidOperationException("Cannot assign courier to delivered shipment");

        
        var courier = await _userManager.FindByIdAsync(courierId);
        if (courier is null)
            throw new InvalidOperationException("Courier not found");

        var isCourier = await _userManager.IsInRoleAsync(courier, Roles.Courier);
        if (!isCourier)
            throw new InvalidOperationException("User is not in Courier role");

        shipment.CourierId = courierId;
        shipment.UpdatedAtUtc = DateTimeOffset.UtcNow;

        var ev = new ShipmentEvent
        {
            ShipmentId = shipment.Id,
            OldStatus = shipment.Status,
            NewStatus = shipment.Status,
            PerformedByUserId = performedByUserId,
            PerformedByRole = performedByRole,
            Notes = $"Assigned courier: {courierId}",
            OccurredAtUtc = DateTimeOffset.UtcNow
        };

        shipment.Events.Add(ev);

        await _db.SaveChangesAsync();

        return MapDetails(shipment);
    }

    

    public async Task<ShipmentDetailsDto> GetDetailsForClientAsync(int shipmentId, string clientId)
    {
        var shipment = await _db.Shipments
            .AsNoTracking()
            .Include(s => s.Events)
            .FirstOrDefaultAsync(s => s.Id == shipmentId);

        if (shipment is null)
            throw new KeyNotFoundException("Shipment not found");

        if (shipment.ClientId != clientId)
            throw new UnauthorizedAccessException("Client has no access to this shipment");

        return MapDetails(shipment);
    }

    public async Task<ShipmentDetailsDto> GetDetailsForCourierAsync(int shipmentId, string courierId)
    {
        var shipment = await _db.Shipments
            .AsNoTracking()
            .Include(s => s.Events)
            .FirstOrDefaultAsync(s => s.Id == shipmentId);

        if (shipment is null)
            throw new KeyNotFoundException("Shipment not found");

        if (shipment.CourierId != courierId)
            throw new UnauthorizedAccessException("Courier has no access to this shipment");

        return MapDetails(shipment);
    }

    public async Task<ShipmentDetailsDto> GetDetailsForAdminAsync(int shipmentId)
    {
        var shipment = await _db.Shipments
            .AsNoTracking()
            .Include(s => s.Events)
            .FirstOrDefaultAsync(s => s.Id == shipmentId);

        if (shipment is null)
            throw new KeyNotFoundException("Shipment not found");

        return MapDetails(shipment);
    }

    

    public async Task<ShipmentDetailsDto> CancelAsync(int shipmentId, CancelShipmentRequest request, string clientId)
    {
        var shipment = await _db.Shipments
            .Include(s => s.Events)
            .FirstOrDefaultAsync(s => s.Id == shipmentId);

        if (shipment is null)
            throw new KeyNotFoundException("Shipment not found");

        if (shipment.ClientId != clientId)
            throw new UnauthorizedAccessException("Client has no access to this shipment");

        var oldStatus = shipment.Status;
        var newStatus = ShipmentStatus.Canceled;

        if (!ShipmentStatusTransitions.CanTransition(oldStatus, newStatus))
        {
            var allowed = ShipmentStatusTransitions.GetAllowedTransitions(oldStatus);
            throw new InvalidOperationException(
                $"Cannot cancel shipment in status {oldStatus}. Allowed: {string.Join(", ", allowed)}");
        }

        shipment.Status = newStatus;
        shipment.UpdatedAtUtc = DateTimeOffset.UtcNow;

        var ev = new ShipmentEvent
        {
            ShipmentId = shipment.Id,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            PerformedByUserId = clientId,
            PerformedByRole = Roles.Client,
            Notes = string.IsNullOrWhiteSpace(request?.Notes) ? "Canceled by client" : request.Notes,
            OccurredAtUtc = DateTimeOffset.UtcNow
        };

        shipment.Events.Add(ev);

        await _db.SaveChangesAsync();

        return MapDetails(shipment);
    }

    

    public async Task<List<ShipmentListItemDto>> GetForClientAsync(string clientId)
    {
        var list = await _db.Shipments.AsNoTracking()
            .Where(s => s.ClientId == clientId)
            .Select(MapListItemExpr)
            .ToListAsync();

        return list.OrderByDescending(x => x.CreatedAtUtc).ToList();
    }

    public async Task<List<ShipmentListItemDto>> GetForCourierAsync(string courierId)
    {
        var list = await _db.Shipments.AsNoTracking()
            .Where(s => s.CourierId == courierId)
            .Select(MapListItemExpr)
            .ToListAsync();

        return list.OrderByDescending(x => x.UpdatedAtUtc).ToList();
    }

    public async Task<List<ShipmentListItemDto>> GetAllAsync()
    {
        var list = await _db.Shipments.AsNoTracking()
            .Select(MapListItemExpr)
            .ToListAsync();

        return list.OrderByDescending(x => x.CreatedAtUtc).ToList();
    }

   

    public async Task<List<CourierListItemDto>> GetCouriersAsync(bool onlyFree)
    {
        var couriers = await _userManager.GetUsersInRoleAsync(Roles.Courier);
        var courierIds = couriers.Select(c => c.Id).ToList();

        var activeCounts = await _db.Shipments.AsNoTracking()
            .Where(s => s.CourierId != null && courierIds.Contains(s.CourierId))
            .Where(s => s.Status != ShipmentStatus.Delivered && s.Status != ShipmentStatus.Canceled)
            .GroupBy(s => s.CourierId!)
            .Select(g => new { CourierId = g.Key, Count = g.Count() })
            .ToListAsync();

        var dict = activeCounts.ToDictionary(x => x.CourierId, x => x.Count);

        var result = couriers.Select(c =>
        {
            dict.TryGetValue(c.Id, out var cnt);
            return new CourierListItemDto
            {
                Id = c.Id,
                Email = c.Email ?? c.UserName ?? c.Id,
                ActiveShipmentsCount = cnt,
                IsFree = cnt == 0
            };
        }).ToList();

        if (onlyFree)
            result = result.Where(x => x.IsFree).ToList();

        return result;
    }

   

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
