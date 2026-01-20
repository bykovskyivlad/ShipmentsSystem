namespace Shipments.Mvc.Services;

public interface ITokenCookieService
{
    void SetToken(HttpResponse response, string token);
    string? GetToken(HttpRequest request);
    void ClearToken(HttpResponse response);
}

public class TokenCookieService : ITokenCookieService
{
    private const string CookieName = "shipments_jwt";

    public void SetToken(HttpResponse response, string token)
        => response.Cookies.Append(CookieName, token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax
        });

    public string? GetToken(HttpRequest request)
        => request.Cookies.TryGetValue(CookieName, out var token) ? token : null;

    public void ClearToken(HttpResponse response)
        => response.Cookies.Delete(CookieName);
}