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

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateShipmentRequest request)
    {
        
        var userId = "TEMP-USER-ID";

        var result = await _shipmentService.CreateAsync(request, userId);
        return Ok(result);
    }
}
