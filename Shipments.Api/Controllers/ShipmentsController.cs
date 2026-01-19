using Microsoft.AspNetCore.Mvc;
using Shipments.Api.Services;
using Shipments.Shared.Contracts.Shipments.Requests;

namespace Shipments.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShipmentsController : ControllerBase
{
    private readonly IShipmentService _shipmentService;

    public ShipmentsController(IShipmentService shipmentService)
    {
        _shipmentService = shipmentService;
    }
    [HttpPatch("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id, [FromBody] CancelShipmentRequest request)
    {
        var userId = "TEMP-USER-ID";
        var dto = await _shipmentService.CancelAsync(id, request, userId);
        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateShipmentRequest request)
    {
        
        var userId = "TEMP-USER-ID";

        var result = await _shipmentService.CreateAsync(request, userId);
        return Ok(result);
    }
}
