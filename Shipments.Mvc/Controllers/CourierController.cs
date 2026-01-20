using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipments.Shared.Contracts.Couriers.Requests;
using Shipments.Shared.Contracts.Shipments.Requests;
using Shipments.Shared.Contracts.Shipments.Responses;
using Shipments.Shared.Domain.Enums;

namespace Shipments.Mvc.Controllers;

[Authorize(Roles = "Courier")]
public class CourierController : Controller
{
    private readonly ApiClient _api;

    public CourierController(ApiClient api)
    {
        _api = api;
    }

    public async Task<IActionResult> Index()
    {
        var shipments = await _api.GetAsync<List<ShipmentListItemDto>>(
            "/api/courier/shipments"
        );

        return View(shipments);
    }

    public async Task<IActionResult> Details(int id)
    {
        var shipment = await _api.GetAsync<ShipmentDetailsDto>(
            $"/api/courier/shipments/{id}"
        );

        return View(shipment);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus(
        int id,
        ShipmentStatus newStatus,
        string? notes)
    {
        var request = new UpdateShipmentStatusRequest
        {
            NewStatus = newStatus,
            Notes = notes
        };

        await _api.PatchAsync(
            $"/api/shipments/{id}/status",
            request
        );

        return RedirectToAction(nameof(Details), new { id });
    }
}