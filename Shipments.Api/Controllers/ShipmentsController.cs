using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Shipments.Shared.Auth;
using Shipments.Shared.Contracts.Shipments.Requests;

namespace Shipments.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShipmentsController : ControllerBase
    {
        [HttpPost]
        
        public IActionResult Create([FromBody] CreateShipmentRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("UserId not found");
            }
            return Ok(new { Message = "works", UserId = userId, Request = request });
        }
    }
}
