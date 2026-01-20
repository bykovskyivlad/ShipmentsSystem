using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipments.Shared.Contracts.Admin;
using Shipments.Shared.Contracts.Couriers.Requests;
using Shipments.Shared.Contracts.Shipments.Requests;
using Shipments.Shared.Contracts.Shipments.Responses;
using Shipments.Shared.Domain.Enums;

namespace Shipments.Mvc.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApiClient _api;

    public AdminController(ApiClient api)
    {
        _api = api;
    }

    public async Task<IActionResult> Index()
    {
        var shipments = await _api.GetAsync<List<ShipmentListItemDto>>(
            "/api/admin/shipments"
        );

        return View(shipments);
    }

    public async Task<IActionResult> Details(int id)
    {
        var shipment = await _api.GetAsync<ShipmentDetailsDto>(
            $"/api/admin/shipments/{id}"
        );

        return View(shipment);
    }

    public async Task<IActionResult> Couriers(int shipmentId, bool onlyFree = false)
    {
        ViewBag.ShipmentId = shipmentId;

        var couriers = await _api.GetAsync<List<CourierListItemDto>>(
            $"/api/admin/couriers?onlyFree={onlyFree}"
        );

        return View(couriers);
    }

    [HttpPost]
    public async Task<IActionResult> AssignCourier(int shipmentId, string courierId)
    {
        await _api.PostAsync<ShipmentDetailsDto>(
            $"/api/shipments/{shipmentId}/assign",
            new AssignCourierRequest
            {
                CourierId = courierId
            }
        );

        return RedirectToAction(nameof(Details), new { id = shipmentId });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus(
        int shipmentId,
        ShipmentStatus newStatus,
        string? notes)
    {
        var req = new UpdateShipmentStatusRequest
        {
            NewStatus = newStatus,
            Notes = notes
        };

        await _api.PatchAsync(
            $"/api/shipments/{shipmentId}/status",
            req
        );

        return RedirectToAction(nameof(Details), new { id = shipmentId });
    }
}