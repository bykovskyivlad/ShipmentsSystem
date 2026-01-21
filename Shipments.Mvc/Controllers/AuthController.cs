using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Shipments.Shared.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace Shipments.Mvc.Controllers;

public class AuthController : Controller
{
    private readonly ApiClient _api;

    public AuthController(ApiClient api)
    {
        _api = api;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterRequest("", "", Roles.Client));
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return View(request);

        try
        {
            await _api.PostAsync(
                "/api/auth/register",
                request
            );
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(request);
        }

        return RedirectToAction("Login");
    }

    [HttpPost]
    public async Task<IActionResult> Login(
        string email,
        string password,
        string? returnUrl = null)
    {
        JsonElement response;

        try
        {
            response = await _api.PostAsync<JsonElement>(
                "/api/auth/login",
                new { Email = email, Password = password }
            );
        }
        catch
        {
            ModelState.AddModelError("", "Invalid email or password");
            return View();
        }

        var token = response.GetProperty("token").GetString();
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token!);

        var claims = jwt.Claims.ToList();
        claims.Add(new Claim("access_token", token!));

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity)
        );

        return RedirectAfterLogin(claims, returnUrl);
    }

    private IActionResult RedirectAfterLogin(
        List<Claim> claims,
        string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl))
            return Redirect(returnUrl);

        var role = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

        return role switch
        {
            Roles.Client => RedirectToAction("Index", "Shipments"),
            Roles.Courier => RedirectToAction("Index", "Courier"),
            Roles.Admin => RedirectToAction("Index", "Admin"),
            _ => RedirectToAction("Index", "Home")
        };
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Login");
    }
}
