using System.ComponentModel;
using IntervalsIcu.Client;
using ModelContextProtocol.Server;

namespace IntervalsIcu.Mcp.Tools;

/// <summary>
/// Per-sport training zones and thresholds (FTP, LTHR, threshold pace, power/HR/pace zones).
/// Essential context for a coaching agent to prescribe intensity correctly.
/// </summary>
[McpServerToolType]
public static class SportSettingsTools
{
    [McpServerTool(Name = "intervals_list_sport_settings"), Description(
        "List sport-specific settings for the athlete: FTP, W', Pmax, power/HR/pace zones, threshold " +
        "pace, warmup/cooldown defaults — one entry per sport (Ride, Run, Swim, etc.).")]
    public static async Task<string> ListSportSettings(
        IntervalsClient client,
        [Description("Athlete id. Use '0' for the API key owner.")] string athleteId = "0",
        CancellationToken ct = default)
    {
        var result = await client.GetAsync($"/api/v1/athlete/{AthleteTools.Id(client, athleteId)}/sport-settings", ct: ct);
        return result.ToPrettyJson();
    }

    [McpServerTool(Name = "intervals_get_sport_settings"), Description("Get sport settings (zones/thresholds) for one sport-settings id.")]
    public static async Task<string> GetSportSettings(
        IntervalsClient client,
        [Description("Sport-settings id (from intervals_list_sport_settings).")] long sportSettingsId,
        [Description("Athlete id. Use '0' for the API key owner.")] string athleteId = "0",
        CancellationToken ct = default)
    {
        var result = await client.GetAsync($"/api/v1/athlete/{AthleteTools.Id(client, athleteId)}/sport-settings/{sportSettingsId}", ct: ct);
        return result.ToPrettyJson();
    }

    [McpServerTool(Name = "intervals_update_sport_settings"), Description(
        "Update sport settings (e.g. FTP after a new test, or power/HR zone boundaries). Body is a " +
        "JSON object with SportSettings schema fields, e.g. {\"ftp\": 250, \"lthr\": 168}.")]
    public static async Task<string> UpdateSportSettings(
        IntervalsClient client,
        [Description("Sport-settings id (from intervals_list_sport_settings).")] long sportSettingsId,
        [Description("JSON object with the fields to update.")] string bodyJson,
        [Description("Athlete id. Use '0' for the API key owner.")] string athleteId = "0",
        CancellationToken ct = default)
    {
        var body = JsonHelpers.ParseOptionalBody(bodyJson) ?? throw new ArgumentException("bodyJson is required");
        var result = await client.PutAsync($"/api/v1/athlete/{AthleteTools.Id(client, athleteId)}/sport-settings/{sportSettingsId}", body, ct: ct);
        return result.ToPrettyJson();
    }
}
