using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipments.Api.Services;
using Shipments.Shared.Auth;
using Shipments.Shared.Contracts.Couriers.Requests;
using System.Security.Claims;

namespace Shipments.Api.Controllers;

[ApiController]
[Route("api/courier/shipments")]
[Authorize(Roles = Roles.Courier)]
public class CourierShipmentsController : ControllerBase
{
    private readonly IShipmentService _service;

    public CourierShipmentsController(IShipmentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> MyAssigned()
    {
        var courierId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (courierId is null) return Unauthorized("UserId not found");

        var list = await _service.GetForCourierAsync(courierId);
        return Ok(list);
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var courierId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (courierId is null) return Unauthorized("UserId not found");

        var dto = await _service.GetDetailsForCourierAsync(id, courierId);
        return Ok(dto);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateShipmentStatusRequest request)
    {
        var courierId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (courierId is null) return Unauthorized("UserId not found");

        var dto = await _service.UpdateStatusAsync(id, request, courierId, Roles.Courier);
        return Ok(dto);
    }
}