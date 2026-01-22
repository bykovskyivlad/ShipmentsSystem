using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shipments.Api.Models;
using Shipments.Api.Services;
using Shipments.Shared.Auth;
using System.Security.Claims;

namespace Shipments.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;

    public AuthController(
        UserManager<AppUser> userManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }


    public record RegisterRequest(string Email, string Password, string Role);
    public record LoginRequest(string Email, string Password);
    public record ChangePasswordRequest(string OldPassword, string NewPassword);

   

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequest req)
    {
        if (req.Role != Roles.Client && req.Role != Roles.Courier)
            return BadRequest("Invalid role");

        var user = new AppUser
        {
            UserName = req.Email,
            Email = req.Email,
            MustChangePassword = false
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
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user is null)
            return Unauthorized("Invalid credentials");

        var ok = await _userManager.CheckPasswordAsync(user, req.Password);
        if (!ok)
            return Unauthorized("Invalid credentials");

        var token = await _tokenService.CreateTokenAsync(user);

        return Ok(new
        {
            token,
            mustChangePassword = user.MustChangePassword
        });
    }

   

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest req)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Unauthorized();

        var result = await _userManager.ChangePasswordAsync(
            user,
            req.OldPassword,
            req.NewPassword
        );

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        
        if (user.MustChangePassword)
        {
            user.MustChangePassword = false;
            await _userManager.UpdateAsync(user);
        }

        return Ok();
    }
}
