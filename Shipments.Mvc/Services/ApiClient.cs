using System.Net.Http.Headers;
using System.Net.Http.Json;

public class ApiClient
{
    private readonly IHttpClientFactory _factory;
    private readonly IHttpContextAccessor _http;

    public ApiClient(IHttpClientFactory factory, IHttpContextAccessor http)
    {
        _factory = factory;
        _http = http;
    }

    private HttpClient CreateClient()
    {
        var client = _factory.CreateClient("Api");

        var token = _http.HttpContext?
            .User?
            .FindFirst("access_token")?
            .Value;

        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }

    public async Task<T?> GetAsync<T>(string url)
    {
        var client = CreateClient();
        return await client.GetFromJsonAsync<T>(url);
    }

    public async Task<T?> PostAsync<T>(string url, object body)
    {
        var client = CreateClient();
        var res = await client.PostAsJsonAsync(url, body);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<T>();
    }

    public async Task PatchAsync(string url, object body)
    {
        var client = CreateClient();
        var res = await client.PatchAsJsonAsync(url, body);
        res.EnsureSuccessStatusCode();
    }
}