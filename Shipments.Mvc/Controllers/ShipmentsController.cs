using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipments.Shared.Contracts.Shipments.Requests;
using Shipments.Shared.Contracts.Shipments.Responses;

[Authorize(Roles = "Client")]
public class ShipmentsController : Controller
{
    private readonly ApiClient _api;

    public ShipmentsController(ApiClient api)
    {
        _api = api;
    }

    public async Task<IActionResult> Index()
    {
        var shipments = await _api.GetAsync<List<ShipmentListItemDto>>(
            "/api/shipments/mine"
        );

        return View(shipments);
    }

    public IActionResult Create()
    {
        return View(new CreateShipmentRequest());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateShipmentRequest request)
    {
        if (!ModelState.IsValid)
            return View(request);

        await _api.PostAsync<ShipmentDetailsDto>(
            "/api/shipments",
            request
        );

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var shipment = await _api.GetAsync<ShipmentDetailsDto>(
            $"/api/shipments/{id}"
        );

        return View(shipment);
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int id)
    {
        await _api.PatchAsync(
            $"/api/shipments/{id}/cancel",
            new { Notes = "Canceled from MVC" }
        );

        return RedirectToAction(nameof(Details), new { id });
    }
}
