using System.ComponentModel;
using IntervalsIcu.Client;
using ModelContextProtocol.Server;

namespace IntervalsIcu.Mcp.Tools;

[McpServerToolType]
public static class AthleteTools
{
    [McpServerTool(Name = "intervals_get_athlete"), Description(
        "Get profile/settings for an Intervals.ICU athlete: name, timezone, sex, weight, resting HR, " +
        "sport settings summary, connected devices, etc. Use athleteId '0' (default) for the athlete " +
        "who owns the configured API key.")]
    public static async Task<string> GetAthlete(
        IntervalsClient client,
        [Description("Athlete id, e.g. 'i12345'. Use '0' for the API key owner.")] string athleteId = "0",
        CancellationToken ct = default)
    {
        var result = await client.GetAsync($"/api/v1/athlete/{Id(client, athleteId)}", ct: ct);
        return result.ToPrettyJson();
    }

    [McpServerTool(Name = "intervals_update_athlete"), Description(
        "Update athlete profile fields (e.g. weight, timezone, measurement preference). " +
        "Body is a JSON object with any subset of the Athlete schema fields from openapi-spec.json.")]
    public static async Task<string> UpdateAthlete(
        IntervalsClient client,
        [Description("JSON object with the athlete fields to update, e.g. {\"weight\": 72.5}")] string bodyJson,
        [Description("Athlete id, e.g. 'i12345'. Use '0' for the API key owner.")] string athleteId = "0",
        CancellationToken ct = default)
    {
        var body = JsonHelpers.ParseOptionalBody(bodyJson) ?? throw new ArgumentException("bodyJson is required");
        var result = await client.PutAsync($"/api/v1/athlete/{Id(client, athleteId)}", body, ct: ct);
        return result.ToPrettyJson();
    }

    [McpServerTool(Name = "intervals_get_athlete_profile"), Description(
        "Get the public-facing athlete profile page data (bio, stats, recent activity summary).")]
    public static async Task<string> GetAthleteProfile(
        IntervalsClient client,
        [Description("Athlete id, e.g. 'i12345'. Use '0' for the API key owner.")] string athleteId = "0",
        CancellationToken ct = default)
    {
        var result = await client.GetAsync($"/api/v1/athlete/{Id(client, athleteId)}/profile", ct: ct);
        return result.ToPrettyJson();
    }

    [McpServerTool(Name = "intervals_get_athlete_summary"), Description(
        "Get the athlete-summary widget data: current fitness (CTL), fatigue (ATL), form (TSB), " +
        "eFTP, and recent training load trend. Great starting point for a coaching agent to assess " +
        "an athlete's current state.")]
    public static async Task<string> GetAthleteSummary(
        IntervalsClient client,
        [Description("Athlete id, e.g. 'i12345'. Use '0' for the API key owner.")] string athleteId = "0",
        [Description("Response extension, usually empty. Leave blank unless you need a specific format.")] string ext = "",
        CancellationToken ct = default)
    {
        var result = await client.GetAsync($"/api/v1/athlete/{Id(client, athleteId)}/athlete-summary{ext}", ct: ct);
        return result.ToPrettyJson();
    }

    [McpServerTool(Name = "intervals_list_athletes"), Description(
        "List athletes visible to the configured API key (e.g. athletes coached by this account).")]
    public static async Task<string> ListAthletes(IntervalsClient client, CancellationToken ct = default)
    {
        var result = await client.GetAsync("/api/v1/athletes", ct: ct);
        return result.ToPrettyJson();
    }

    internal static string Id(IntervalsClient client, string athleteId) =>
        string.IsNullOrWhiteSpace(athleteId) ? client.DefaultAthleteId : athleteId;
}
