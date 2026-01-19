using Shipments.Shared.Contracts.Couriers.Requests;
using Shipments.Shared.Contracts.Shipments.Requests;
using Shipments.Shared.Contracts.Shipments.Responses;

namespace Shipments.Api.Services;

public interface IShipmentService
{
    Task<ShipmentDetailsDto> CreateAsync(CreateShipmentRequest request, string userId);

    Task<ShipmentDetailsDto> UpdateStatusAsync(int shipmentId, UpdateShipmentStatusRequest request, string performedByUserId, string performedByRole);
    Task<ShipmentDetailsDto> AssignCourierAsync(int shipmentId, string courierId, string performedByUserId, string performedByRole);

    Task<List<ShipmentListItemDto>> GetForClientAsync(string clientId);
    Task<List<ShipmentListItemDto>> GetForCourierAsync(string courierId);
    Task<List<ShipmentListItemDto>> GetAllAsync();

}