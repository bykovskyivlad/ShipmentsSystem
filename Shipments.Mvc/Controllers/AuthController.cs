using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Shipments.Mvc.Models;
using Shipments.Mvc.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Shipments.Mvc.Controllers;

public class AuthController : Controller
{
    private readonly ShipmentsApiClient _api;
    private readonly ITokenCookieService _tokens;

    public AuthController(ShipmentsApiClient api, ITokenCookieService tokens)
    {
        _api = api;
        _tokens = tokens;
    }

    public record LoginRequest(string Email, string Password);
    public record LoginResponse(string Token);

    [HttpGet]
    public IActionResult Login() => View(new LoginViewModel());

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var resp = await _api.PostAsync<LoginRequest, LoginResponse>("/api/auth/login", new(model.Email, model.Password));
        if (resp is null || string.IsNullOrWhiteSpace(resp.Token))
        {
            ModelState.AddModelError("", "Login failed");
            return View(model);
        }

        _tokens.SetToken(Response, resp.Token);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(resp.Token);
        var claims = jwt.Claims.ToList();

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        _tokens.ClearToken(Response);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    public IActionResult Denied() => View();
}
