using Microsoft.AspNetCore.Mvc;
using Shipments.Api.Services;
using Shipments.Shared.Auth;
using Shipments.Shared.Contracts.Couriers.Requests;

namespace Shipments.Api.Controllers;

[ApiController]
[Route("api/admin/shipments")]
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

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateShipmentStatusRequest request)
    {
        var adminId = "TEMP-ADMIN-ID";
        var dto = await _service.UpdateStatusAsync(id, request, adminId, Roles.Admin);
        return Ok(dto);
    }
}
