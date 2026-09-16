using System.ComponentModel;
using System.Text.Json;
using IntervalsIcu.Client;
using ModelContextProtocol.Server;

namespace IntervalsIcu.Mcp.Tools;

[McpServerToolType]
public static class ActivityTools
{
    [McpServerTool(Name = "intervals_list_activities"), Description(
        "List an athlete's activities (rides, runs, swims, etc.) in a date range, including load, " +
        "duration, distance, and fitness metrics per activity (icu_training_load, icu_ctl, icu_atl).")]
    public static async Task<string> ListActivities(
        IntervalsClient client,
        [Description("Oldest start date, yyyy-MM-dd (inclusive).")] string oldest,
        [Description("Newest start date, yyyy-MM-dd (inclusive). Defaults to today if omitted.")] string? newest = null,
        [Description("Max number of activities to return.")] int? limit = null,
        [Description("Athlete id. Use '0' for the API key owner.")] string athleteId = "0",
        CancellationToken ct = default)
    {
        var query = JsonHelpers.Query(("oldest", oldest), ("newest", newest), ("limit", limit?.ToString()));
        var result = await client.GetAsync($"/api/v1/athlete/{AthleteTools.Id(client, athleteId)}/activities", query, ct);
        return result.ToPrettyJson();
    }

    [McpServerTool(Name = "intervals_search_activities"), Description(
        "Full-text/filtered search over an athlete's activities using Intervals.ICU's search query " +
        "syntax (see 'Search' help in the Intervals.ICU UI), e.g. 'type=Ride and icu_training_load > 80'.")]
    public static async Task<string> SearchActivities(
        IntervalsClient client,
        [Description("Intervals.ICU search query string.")] string query,
        [Description("Athlete id. Use '0' for the API key owner.")] string athleteId = "0",
        CancellationToken ct = default)
    {
        var q = JsonHelpers.Query(("query", query));
        var result = await client.GetAsync($"/api/v1/athlete/{AthleteTools.Id(client, athleteId)}/activities/search", q, ct);
        return result.ToPrettyJson();
    }

    [McpServerTool(Name = "intervals_get_activity"), Description(
        "Get full details for a single activity by id, including all computed metrics (power/HR/pace " +
        "curves summary, training load, elevation, weather, etc.).")]
    public static async Task<string> GetActivity(
        IntervalsClient client,
        [Description("Activity id, e.g. 'i12345678'.")] string activityId,
        CancellationToken ct = default)
    {
        var result = await client.GetAsync($"/api/v1/activity/{activityId}", ct: ct);
        return result.ToPrettyJson();
    }

    [McpServerTool(Name = "intervals_get_activity_intervals"), Description(
        "Get the detected/edited intervals (laps, repeats, work/rest segments) for an activity, with " +
        "average power/HR/pace/cadence per interval. Useful for analysing how a workout was executed.")]
    public static async Task<string> GetActivityIntervals(
        IntervalsClient client,
        [Description("Activity id, e.g. 'i12345678'.")] string activityId,
        CancellationToken ct = default)
    {
        var result = await client.GetAsync($"/api/v1/activity/{activityId}/intervals", ct: ct);
        return result.ToPrettyJson();
    }

    [McpServerTool(Name = "intervals_get_activity_best_efforts"), Description(
        "Get an activity's best-effort power/pace/HR bests (e.g. best 5min power, best 1km pace) " +
        "used to compare against the athlete's all-time curves.")]
    public static async Task<string> GetActivityBestEfforts(
        IntervalsClient client,
        [Description("Activity id, e.g. 'i12345678'.")] string activityId,
        CancellationToken ct = default)
    {
        var result = await client.GetAsync($"/api/v1/activity/{activityId}/best-efforts", ct: ct);
        return result.ToPrettyJson();
    }

    [McpServerTool(Name = "intervals_get_activity_streams"), Description(
        "Get raw time-series data streams for an activity (watts, heartrate, cadence, altitude, " +
        "velocity_smooth, latlng, etc). Can be a large payload; prefer requesting only needed types.")]
    public static async Task<string> GetActivityStreams(
        IntervalsClient client,
        [Description("Activity id, e.g. 'i12345678'.")] string activityId,
        [Description("Comma-separated stream types to include, e.g. 'watts,heartrate,cadence'. Omit for all.")] string? types = null,
        CancellationToken ct = default)
    {
        var query = JsonHelpers.Query(("types", types));
        var result = await client.GetAsync($"/api/v1/activity/{activityId}/streams", query, ct);
        return result.ToPrettyJson();
    }

    [McpServerTool(Name = "intervals_get_power_curve"), Description(
        "Get the athlete's all-time (or windowed) mean-maximal power curve across all rides — the " +
        "core input for estimating FTP/CP/W' and setting power-based training zones.")]
    public static async Task<string> GetPowerCurve(
        IntervalsClient client,
        [Description("Oldest date to include, yyyy-MM-dd. Omit for all-time.")] string? oldest = null,
        [Description("Newest date to include, yyyy-MM-dd. Omit for all-time.")] string? newest = null,
        [Description("Athlete id. Use '0' for the API key owner.")] string athleteId = "0",
        CancellationToken ct = default)
    {
        var query = JsonHelpers.Query(("oldest", oldest), ("newest", newest));
        var result = await client.GetAsync($"/api/v1/athlete/{AthleteTools.Id(client, athleteId)}/power-curves", query, ct);
        return result.ToPrettyJson();
    }

    [McpServerTool(Name = "intervals_get_pace_curve"), Description(
        "Get the athlete's all-time (or windowed) mean-maximal pace curve across all runs/swims — " +
        "used to estimate running/swimming threshold pace and set pace-based training zones.")]
    public static async Task<string> GetPaceCurve(
        IntervalsClient client,
        [Description("Oldest date to include, yyyy-MM-dd. Omit for all-time.")] string? oldest = null,
        [Description("Newest date to include, yyyy-MM-dd. Omit for all-time.")] string? newest = null,
        [Description("Athlete id. Use '0' for the API key owner.")] string athleteId = "0",
        CancellationToken ct = default)
    {
        var query = JsonHelpers.Query(("oldest", oldest), ("newest", newest));
        var result = await client.GetAsync($"/api/v1/athlete/{AthleteTools.Id(client, athleteId)}/pace-curves", query, ct);
        return result.ToPrettyJson();
    }

    [McpServerTool(Name = "intervals_get_hr_curve"), Description(
        "Get the athlete's all-time (or windowed) mean-maximal heart-rate curve across activities.")]
    public static async Task<string> GetHrCurve(
        IntervalsClient client,
        [Description("Oldest date to include, yyyy-MM-dd. Omit for all-time.")] string? oldest = null,
        [Description("Newest date to include, yyyy-MM-dd. Omit for all-time.")] string? newest = null,
        [Description("Athlete id. Use '0' for the API key owner.")] string athleteId = "0",
        CancellationToken ct = default)
    {
        var query = JsonHelpers.Query(("oldest", oldest), ("newest", newest));
        var result = await client.GetAsync($"/api/v1/athlete/{AthleteTools.Id(client, athleteId)}/hr-curves", query, ct);
        return result.ToPrettyJson();
    }

    [McpServerTool(Name = "intervals_list_activity_comments"), Description(
        "List existing comments (coach and athlete) on an activity's comment thread — check this " +
        "before adding a new coach note, so you don't repeat feedback already given.")]
    public static async Task<string> ListActivityComments(
        IntervalsClient client,
        [Description("Activity id, e.g. 'i12345678'.")] string activityId,
        CancellationToken ct = default)
    {
        var result = await client.GetAsync($"/api/v1/activity/{activityId}/messages", ct: ct);
        return result.ToPrettyJson();
    }

    [McpServerTool(Name = "intervals_add_activity_comment"), Description(
        "Add a coach note/comment to a completed activity — it appears in that activity's comment " +
        "thread on Intervals.ICU, visible to the athlete. Use for brief, specific coaching feedback " +
        "tied to that session (e.g. cadence, pacing, breathing, a specific interval that stood out). " +
        "Keep it to one short paragraph, about 255 characters — this is a quick tip, not a report.")]
    public static async Task<string> AddActivityComment(
        IntervalsClient client,
        [Description("Activity id, e.g. 'i12345678'.")] string activityId,
        [Description("Comment text. Keep it brief — around 255 characters, one short paragraph.")] string content,
        CancellationToken ct = default)
    {
        var body = JsonSerializer.SerializeToElement(new { content });
        var result = await client.PostAsync($"/api/v1/activity/{activityId}/messages", body, ct: ct);
        return result.ToPrettyJson();
    }
}
