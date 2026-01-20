using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Shipments.Mvc.Services;

public class ShipmentsApiClient
{
    private readonly IHttpClientFactory _factory;
    private readonly IHttpContextAccessor _http;
    private readonly ITokenCookieService _tokenCookies;

    public ShipmentsApiClient(IHttpClientFactory factory, IHttpContextAccessor http, ITokenCookieService tokenCookies)
    {
        _factory = factory;
        _http = http;
        _tokenCookies = tokenCookies;
    }

    private HttpClient Create()
    {
        var client = _factory.CreateClient("ShipmentsApi");
        var token = _tokenCookies.GetToken(_http.HttpContext!.Request);

        if (!string.IsNullOrWhiteSpace(token))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return client;
    }

    public async Task<T?> GetAsync<T>(string url)
        => await Create().GetFromJsonAsync<T>(url);

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest body)
    {
        var resp = await Create().PostAsJsonAsync(url, body);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<TResponse>();
    }

    public async Task<TResponse?> PatchAsync<TRequest, TResponse>(string url, TRequest body)
    {
        var req = new HttpRequestMessage(HttpMethod.Patch, url) { Content = JsonContent.Create(body) };
        var resp = await Create().SendAsync(req);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<TResponse>();
    }
}