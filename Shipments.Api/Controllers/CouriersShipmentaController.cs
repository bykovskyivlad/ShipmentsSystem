using Microsoft.AspNetCore.Mvc;
using Shipments.Api.Services;
using Shipments.Shared.Auth;
using Shipments.Shared.Contracts.Couriers.Requests;

namespace Shipments.Api.Controllers;

[ApiController]
[Route("api/courier/shipments")]
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
        var courierId = "TEMP-COURIER-ID";
        var list = await _service.GetForCourierAsync(courierId);
        return Ok(list);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateShipmentStatusRequest request)
    {
        var courierId = "TEMP-COURIER-ID";
        var dto = await _service.UpdateStatusAsync(id, request, courierId, Roles.Courier);
        return Ok(dto);
    }
}