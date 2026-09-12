namespace IntervalsIcu.Client;

/// <summary>
/// Configuration for talking to the Intervals.ICU REST API.
/// Bound from environment variables / appsettings under the "Intervals" section.
/// </summary>
public sealed class IntervalsClientOptions
{
    public const string SectionName = "Intervals";

    /// <summary>Base URL of the Intervals.ICU API.</summary>
    public string BaseUrl { get; set; } = "https://intervals.icu";

    /// <summary>
    /// API key found in the athlete's Intervals.ICU settings page.
    /// Sent as HTTP Basic Auth with username "API_KEY".
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Default athlete id used by tools when the caller does not specify one.
    /// Intervals.ICU accepts "0" as an alias for "the athlete owning the API key".
    /// </summary>
    public string DefaultAthleteId { get; set; } = "0";
}
