using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipments.Api.Services;
using Shipments.Shared.Auth;
using Shipments.Shared.Contracts.Shipments.Requests;
using System.Security.Claims;

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
    [Authorize(Roles = Roles.Client)]
    public async Task<IActionResult> Cancel(int id, [FromBody] CancelShipmentRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized("UserId not found");

        var dto = await _shipmentService.CancelAsync(id, request, userId);
        return Ok(dto);
    }
    [HttpGet("mine")]
    [Authorize(Roles = Roles.Client)]
    public async Task<IActionResult> Mine()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized("UserId not found");

        var list = await _shipmentService.GetForClientAsync(userId);
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = Roles.Client)]
    public async Task<IActionResult> Details(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized("UserId not found");

        var dto = await _shipmentService.GetDetailsForClientAsync(id, userId);
        return Ok(dto);
    }
    [HttpPost]
    [Authorize(Roles = Roles.Client)]
    public async Task<IActionResult> Create([FromBody] CreateShipmentRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized("UserId not found");

        var result = await _shipmentService.CreateAsync(request, userId);
        return Ok(result);
    }
}
