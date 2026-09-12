using System.Text.Json;

namespace IntervalsIcu.Mcp.Tools;

internal static class JsonHelpers
{
    private static readonly JsonSerializerOptions PrettyOptions = new() { WriteIndented = true };

    /// <summary>Pretty-prints a JsonElement so tool results are readable in a chat transcript.</summary>
    public static string ToPrettyJson(this JsonElement element) => JsonSerializer.Serialize(element, PrettyOptions);

    /// <summary>Parses an optional raw JSON string body supplied by a tool caller.</summary>
    public static JsonElement? ParseOptionalBody(string? bodyJson)
    {
        if (string.IsNullOrWhiteSpace(bodyJson)) return null;
        using var doc = JsonDocument.Parse(bodyJson);
        return doc.RootElement.Clone();
    }

    public static Dictionary<string, string?> Query(params (string Key, string? Value)[] pairs)
    {
        var dict = new Dictionary<string, string?>();
        foreach (var (key, value) in pairs)
        {
            if (value is not null) dict[key] = value;
        }
        return dict;
    }
}
