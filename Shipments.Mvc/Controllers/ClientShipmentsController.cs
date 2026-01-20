using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipments.Mvc.Services;
using Shipments.Shared.Auth;
using Shipments.Shared.Contracts.Shipments.Responses;

namespace Shipments.Mvc.Controllers;

[Authorize(Roles = Roles.Client)]
public class ClientShipmentsController : Controller
{
    private readonly ShipmentsApiClient _api;

    public ClientShipmentsController(ShipmentsApiClient api)
    {
        _api = api;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _api.GetAsync<List<ShipmentListItemDto>>("/api/Shipments/mine");
        return View(list ?? new());
    }
}