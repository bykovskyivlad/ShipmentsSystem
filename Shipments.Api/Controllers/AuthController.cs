using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipments.Api.Models;
using Shipments.Api.Services;
using Shipments.Shared.Auth;

namespace Shipments.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;

    public AuthController(UserManager<AppUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public record RegisterRequest(string Email, string Password, string Role);
    public record LoginRequest(string Email, string Password);

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequest req)
    {
        if (req.Role != Roles.Client && req.Role != Roles.Courier)
            return BadRequest("Invalid role");

        var user = new AppUser
        {
            UserName = req.Email,
            Email = req.Email
        };

        var result = await _userManager.CreateAsync(user, req.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        var roleResult = await _userManager.AddToRoleAsync(user, req.Role);
        if (!roleResult.Succeeded)
            return BadRequest(roleResult.Errors);

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user is null) return Unauthorized("Invalid credentials");

        var ok = await _userManager.CheckPasswordAsync(user, req.Password);
        if (!ok) return Unauthorized("Invalid credentials");

        var token = await _tokenService.CreateTokenAsync(user);
        return Ok(new { token });
    }
}