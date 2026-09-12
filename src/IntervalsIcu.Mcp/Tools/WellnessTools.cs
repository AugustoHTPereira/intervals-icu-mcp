using System.ComponentModel;
using IntervalsIcu.Client;
using ModelContextProtocol.Server;

namespace IntervalsIcu.Mcp.Tools;

[McpServerToolType]
public static class WellnessTools
{
    [McpServerTool(Name = "intervals_get_wellness"), Description(
        "Get a single day's wellness entry: CTL/ATL/ramp rate, resting HR, HRV, sleep, weight, " +
        "soreness/fatigue/stress/mood/motivation, menstrual phase, etc.")]
    public static async Task<string> GetWellness(
        IntervalsClient client,
        [Description("Date in yyyy-MM-dd format.")] string date,
        [Description("Athlete id. Use '0' for the API key owner.")] string athleteId = "0",
        CancellationToken ct = default)
    {
        var result = await client.GetAsync($"/api/v1/athlete/{AthleteTools.Id(client, athleteId)}/wellness/{date}", ct: ct);
        return result.ToPrettyJson();
    }

    [McpServerTool(Name = "intervals_list_wellness"), Description(
        "Get a range of daily wellness entries, including CTL/ATL/form (TSB) history. This is the " +
        "primary source for training-load trend analysis, e.g. to decide whether an athlete should " +
        "rest, taper, or can absorb more load.")]
    public static async Task<string> ListWellness(
        IntervalsClient client,
        [Description("Oldest date, yyyy-MM-dd (inclusive).")] string oldest,
        [Description("Newest date, yyyy-MM-dd (inclusive). Defaults to today if omitted.")] string? newest = null,
        [Description("Athlete id. Use '0' for the API key owner.")] string athleteId = "0",
        CancellationToken ct = default)
    {
        var query = JsonHelpers.Query(("oldest", oldest), ("newest", newest));
        var result = await client.GetAsync($"/api/v1/athlete/{AthleteTools.Id(client, athleteId)}/wellness", query, ct);
        return result.ToPrettyJson();
    }

    [McpServerTool(Name = "intervals_upsert_wellness"), Description(
        "Create or update a single day's wellness entry (e.g. log resting HR, HRV, sleep, weight, " +
        "soreness/fatigue/stress/mood after a check-in). Body is a JSON object with any subset of the " +
        "Wellness schema fields from openapi-spec.json (e.g. {\"restingHR\": 48, \"hrv\": 65, \"fatigue\": 2}).")]
    public static async Task<string> UpsertWellness(
        IntervalsClient client,
        [Description("Date in yyyy-MM-dd format.")] string date,
        [Description("JSON object with wellness fields to set for that date.")] string bodyJson,
        [Description("Athlete id. Use '0' for the API key owner.")] string athleteId = "0",
        CancellationToken ct = default)
    {
        var body = JsonHelpers.ParseOptionalBody(bodyJson) ?? throw new ArgumentException("bodyJson is required");
        var result = await client.PutAsync($"/api/v1/athlete/{AthleteTools.Id(client, athleteId)}/wellness/{date}", body, ct: ct);
        return result.ToPrettyJson();
    }

    [McpServerTool(Name = "intervals_bulk_upsert_wellness"), Description(
        "Create or update multiple days of wellness entries in one call. Body is a JSON array of " +
        "Wellness objects, each including an 'id' field with the date (yyyy-MM-dd) it applies to.")]
    public static async Task<string> BulkUpsertWellness(
        IntervalsClient client,
        [Description("JSON array of wellness entries, each with an 'id' date field.")] string bodyJson,
        [Description("Athlete id. Use '0' for the API key owner.")] string athleteId = "0",
        CancellationToken ct = default)
    {
        var body = JsonHelpers.ParseOptionalBody(bodyJson) ?? throw new ArgumentException("bodyJson is required");
        var result = await client.PutAsync($"/api/v1/athlete/{AthleteTools.Id(client, athleteId)}/wellness-bulk", body, ct: ct);
        return result.ToPrettyJson();
    }
}
