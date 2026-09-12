using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace IntervalsIcu.Client;

/// <summary>
/// Thin typed wrapper around the Intervals.ICU REST API (https://intervals.icu/api-docs.html).
/// All methods work with raw <see cref="JsonElement"/> payloads instead of per-endpoint DTOs:
/// the API surface is ~120 endpoints with large, evolving schemas, so tool callers are expected
/// to shape request/response bodies per the fields documented in openapi-spec.json rather than
/// relying on a strongly-typed model for every resource.
/// </summary>
public sealed class IntervalsClient
{
    private readonly HttpClient _http;
    private readonly IntervalsClientOptions _options;

    public IntervalsClient(HttpClient http, IOptions<IntervalsClientOptions> options)
    {
        _options = options.Value;
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            throw new InvalidOperationException(
                "Intervals.ICU API key not configured. Set the INTERVALS_API_KEY environment variable " +
                "(found in the athlete's Intervals.ICU Settings page under 'Developer Settings').");
        }

        http.BaseAddress = new Uri(_options.BaseUrl, UriKind.Absolute);
        var basicAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"API_KEY:{_options.ApiKey}"));
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);
        http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _http = http;
    }

    /// <summary>Athlete id to use when a tool caller does not specify one explicitly.</summary>
    public string DefaultAthleteId => _options.DefaultAthleteId;

    public Task<JsonElement> GetAsync(string path, IReadOnlyDictionary<string, string?>? query = null, CancellationToken ct = default)
        => SendAsync(HttpMethod.Get, path, query, body: null, ct);

    public Task<JsonElement> PostAsync(string path, JsonElement? body = null, IReadOnlyDictionary<string, string?>? query = null, CancellationToken ct = default)
        => SendAsync(HttpMethod.Post, path, query, body, ct);

    public Task<JsonElement> PutAsync(string path, JsonElement? body = null, IReadOnlyDictionary<string, string?>? query = null, CancellationToken ct = default)
        => SendAsync(HttpMethod.Put, path, query, body, ct);

    public Task<JsonElement> DeleteAsync(string path, IReadOnlyDictionary<string, string?>? query = null, CancellationToken ct = default)
        => SendAsync(HttpMethod.Delete, path, query, body: null, ct);

    /// <summary>
    /// Escape hatch used by the generic "raw request" MCP tool: issues an arbitrary request against
    /// any Intervals.ICU endpoint listed in openapi-spec.json, for the long tail of endpoints that
    /// don't have a dedicated, ergonomic tool.
    /// </summary>
    public async Task<(int StatusCode, string Body)> RawRequestAsync(
        string method,
        string path,
        IReadOnlyDictionary<string, string?>? query,
        string? bodyJson,
        CancellationToken ct = default)
    {
        var httpMethod = new HttpMethod(method.ToUpperInvariant());
        using var request = new HttpRequestMessage(httpMethod, BuildUri(path, query));
        if (!string.IsNullOrWhiteSpace(bodyJson))
        {
            request.Content = new StringContent(bodyJson, Encoding.UTF8, "application/json");
        }

        using var response = await _http.SendAsync(request, ct).ConfigureAwait(false);
        var content = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
        return ((int)response.StatusCode, content);
    }

    private async Task<JsonElement> SendAsync(
        HttpMethod method,
        string path,
        IReadOnlyDictionary<string, string?>? query,
        JsonElement? body,
        CancellationToken ct)
    {
        using var request = new HttpRequestMessage(method, BuildUri(path, query));
        if (body is { } b)
        {
            request.Content = new StringContent(b.GetRawText(), Encoding.UTF8, "application/json");
        }

        using var response = await _http.SendAsync(request, ct).ConfigureAwait(false);
        var content = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            throw new IntervalsApiException((int)response.StatusCode, content);
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            return JsonDocument.Parse("{}").RootElement;
        }

        using var doc = JsonDocument.Parse(content);
        return doc.RootElement.Clone();
    }

    private static string BuildUri(string path, IReadOnlyDictionary<string, string?>? query)
    {
        var relativePath = path.StartsWith('/') ? path : "/" + path;
        if (query is null || query.Count == 0)
        {
            return relativePath;
        }

        var pairs = query
            .Where(kv => kv.Value is not null)
            .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value!)}");
        var qs = string.Join('&', pairs);
        return qs.Length == 0 ? relativePath : $"{relativePath}?{qs}";
    }
}
