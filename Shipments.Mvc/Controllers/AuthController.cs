using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
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

    [HttpPost]
    public async Task<IActionResult> Login(
        string email,
        string password,
        string? returnUrl = null)
    {
        var json = await _api.PostAsync<JsonElement>(
            "/api/auth/login",
            new { Email = email, Password = password }
        );

        if (json.ValueKind == JsonValueKind.Undefined)
        {
            ModelState.AddModelError("", "Invalid email or password");
            return View();
        }

        var token = json.GetProperty("token").GetString();
        var role = json.GetProperty("role").GetString();
        var mail = json.GetProperty("email").GetString();

        if (token is null || role is null)
        {
            ModelState.AddModelError("", "Invalid login response");
            return View();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, mail ?? email),
            new Claim(ClaimTypes.Role, role),
            new Claim("access_token", token)
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity)
        );

        return RedirectAfterLogin(role, returnUrl);
    }

    private IActionResult RedirectAfterLogin(string role, string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl))
            return Redirect(returnUrl);

        return role switch
        {
            "Client" => RedirectToAction("Index", "Shipments"),
            "Courier" => RedirectToAction("Index", "Courier"),
            "Admin" => RedirectToAction("Index", "Admin"),
            _ => RedirectToAction("Index", "Home")
        };
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Login");
    }
}