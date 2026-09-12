namespace IntervalsIcu.Client;

/// <summary>Thrown when the Intervals.ICU API returns a non-success status code.</summary>
public sealed class IntervalsApiException : Exception
{
    public int StatusCode { get; }
    public string? ResponseBody { get; }

    public IntervalsApiException(int statusCode, string? responseBody)
        : base($"Intervals.ICU API returned HTTP {statusCode}: {Trim(responseBody)}")
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
    }

    private static string Trim(string? body)
    {
        if (string.IsNullOrEmpty(body)) return "(empty body)";
        return body.Length > 500 ? body[..500] + "…" : body;
    }
}
