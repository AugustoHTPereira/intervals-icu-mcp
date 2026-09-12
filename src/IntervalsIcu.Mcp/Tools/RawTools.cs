using System.ComponentModel;
using IntervalsIcu.Client;
using ModelContextProtocol.Server;

namespace IntervalsIcu.Mcp.Tools;

/// <summary>
/// Generic escape hatch that reaches every Intervals.ICU endpoint, including the many not covered
/// by a dedicated tool above (chat/messages, gear, custom items, weather, routes, activity streams
/// export formats, etc). Consult the bundled openapi-spec.json for exact paths, parameters and
/// request/response schemas before using this.
/// </summary>
[McpServerToolType]
public static class RawTools
{
    [McpServerTool(Name = "intervals_raw_request"), Description(
        "Call any Intervals.ICU API endpoint directly by HTTP method and path, for endpoints not " +
        "covered by a dedicated tool (e.g. chat/messages, gear, custom items, weather forecast/config, " +
        "routes, GPX/FIT export, athlete training plan, activity streams.csv, etc). Look up the exact " +
        "path, query parameters and request body shape in the openapi-spec.json file shipped with this " +
        "server before calling. Paths are relative to https://intervals.icu, e.g. " +
        "'/api/v1/athlete/0/gear' or '/api/v1/chats/123/messages'. Authentication is handled " +
        "automatically using the server's configured API key.")]
    public static async Task<string> RawRequest(
        IntervalsClient client,
        [Description("HTTP method: GET, POST, PUT, DELETE or PATCH.")] string method,
        [Description("API path starting with /api/v1/..., matching openapi-spec.json.")] string path,
        [Description("Optional query string parameters as a JSON object of string key/value pairs, e.g. {\"oldest\": \"2026-01-01\"}.")] string? queryJson = null,
        [Description("Optional request body as a raw JSON string (object or array), for POST/PUT/PATCH.")] string? bodyJson = null,
        CancellationToken ct = default)
    {
        Dictionary<string, string?>? query = null;
        if (!string.IsNullOrWhiteSpace(queryJson))
        {
            using var doc = System.Text.Json.JsonDocument.Parse(queryJson);
            query = doc.RootElement.EnumerateObject()
                .ToDictionary(p => p.Name, p => p.Value.ValueKind == System.Text.Json.JsonValueKind.String
                    ? p.Value.GetString()
                    : p.Value.GetRawText());
        }

        var (statusCode, body) = await client.RawRequestAsync(method, path, query, bodyJson, ct);
        return $"HTTP {statusCode}\n{body}";
    }
}
