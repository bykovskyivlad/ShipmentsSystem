using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipments.Api.Services;
using Shipments.Shared.Auth;
using Shipments.Shared.Contracts.Couriers.Requests;
using System.Security.Claims;

namespace Shipments.Api.Controllers;

[ApiController]
[Route("api/admin/shipments")]
[Authorize(Roles = Roles.Admin)]
public class AdminShipmentsController : ControllerBase
{
    private readonly IShipmentService _service;

    public AdminShipmentsController(IShipmentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> All()
    {
        var list = await _service.GetAllAsync();
        return Ok(list);
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var dto = await _service.GetDetailsForAdminAsync(id);
        return Ok(dto);
    }


    [HttpPatch("{id:int}/assign/{courierId}")]
    public async Task<IActionResult> AssignCourier(int id, string courierId)
    {
        var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (adminId is null) return Unauthorized("UserId not found");

        var dto = await _service.AssignCourierAsync(id, courierId, adminId, Roles.Admin);
        return Ok(dto);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateShipmentStatusRequest request)
    {
        var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (adminId is null) return Unauthorized("UserId not found");

        var dto = await _service.UpdateStatusAsync(id, request, adminId, Roles.Admin);
        return Ok(dto);
    }

    [HttpGet("couriers")]
    public async Task<IActionResult> Couriers([FromQuery] bool onlyFree = false)
    {
        var list = await _service.GetCouriersAsync(onlyFree);
        return Ok(list);
    }

}