using Shipments.Shared.Contracts.Shipments.Requests;
using Shipments.Shared.Contracts.Shipments.Responses;

namespace Shipments.Api.Services
{
    public interface IShipmentService
    {
        Task<ShipmentDetailsDto> CreateAsync(CreateShipmentRequest request,
            string userId);
    }
}
