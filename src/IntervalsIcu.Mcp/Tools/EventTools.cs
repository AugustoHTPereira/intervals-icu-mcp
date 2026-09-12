using System.ComponentModel;
using IntervalsIcu.Client;
using ModelContextProtocol.Server;

namespace IntervalsIcu.Mcp.Tools;

/// <summary>
/// Calendar/planned-workout tools. This is the primary surface a coaching agent uses to schedule
/// future training: an Event with category "WORKOUT" and a workout_doc is what shows up as a
/// planned workout on the athlete's calendar in Intervals.ICU.
/// </summary>
[McpServerToolType]
public static class EventTools
{
    [McpServerTool(Name = "intervals_list_events"), Description(
        "List calendar events (planned workouts, races, notes) in a date range. Category filter " +
        "values include WORKOUT, RACE_A, RACE_B, RACE_C, NOTE, TARGET, FITNESS_DAYS, etc.")]
    public static async Task<string> ListEvents(
        IntervalsClient client,
        [Description("Oldest date, yyyy-MM-dd (inclusive).")] string oldest,
        [Description("Newest date, yyyy-MM-dd (inclusive).")] string? newest = null,
        [Description("Optional category filter, e.g. 'WORKOUT' or 'RACE_A'.")] string? category = null,
        [Description("Athlete id. Use '0' for the API key owner.")] string athleteId = "0",
        CancellationToken ct = default)
    {
        var query = JsonHelpers.Query(("oldest", oldest), ("newest", newest), ("category", category));
        var result = await client.GetAsync($"/api/v1/athlete/{AthleteTools.Id(client, athleteId)}/events", query, ct);
        return result.ToPrettyJson();
    }

    [McpServerTool(Name = "intervals_get_event"), Description("Get a single calendar event/planned workout by id.")]
    public static async Task<string> GetEvent(
        IntervalsClient client,
        [Description("Event id.")] long eventId,
        [Description("Athlete id. Use '0' for the API key owner.")] string athleteId = "0",
        CancellationToken ct = default)
    {
        var result = await client.GetAsync($"/api/v1/athlete/{AthleteTools.Id(client, athleteId)}/events/{eventId}", ct: ct);
        return result.ToPrettyJson();
    }

    [McpServerTool(Name = "intervals_create_event"), Description(
        "Create a calendar event — most importantly a planned workout for a future date, which is " +
        "how a coaching agent schedules training. Body is a JSON object with Event schema fields, " +
        "e.g. {\"start_date_local\": \"2026-09-15T07:00:00\", \"category\": \"WORKOUT\", \"type\": \"Ride\", " +
        "\"name\": \"Sweet Spot 3x12\", \"description\": \"Warmup 15min, 3x12min @ 88-94% FTP w/ 5min recovery, cooldown 10min\", " +
        "\"moving_time\": 4200}. For structured workouts with per-step targets, include a 'workout_doc' " +
        "object (see the Workout schema / Intervals.ICU workout builder format in openapi-spec.json).")]
    public static async Task<string> CreateEvent(
        IntervalsClient client,
        [Description("JSON object with the new event's fields.")] string bodyJson,
        [Description("If true and the body includes a 'uid', update the existing event with that uid instead of creating a duplicate.")] bool upsertOnUid = false,
        [Description("Athlete id. Use '0' for the API key owner.")] string athleteId = "0",
        CancellationToken ct = default)
    {
        var body = JsonHelpers.ParseOptionalBody(bodyJson) ?? throw new ArgumentException("bodyJson is required");
        var query = JsonHelpers.Query(("upsertOnUid", upsertOnUid.ToString().ToLowerInvariant()));
        var result = await client.PostAsync($"/api/v1/athlete/{AthleteTools.Id(client, athleteId)}/events", body, query, ct);
        return result.ToPrettyJson();
    }

    [McpServerTool(Name = "intervals_update_event"), Description(
        "Update an existing calendar event/planned workout by id. Body is a JSON object with the " +
        "Event fields to change.")]
    public static async Task<string> UpdateEvent(
        IntervalsClient client,
        [Description("Event id.")] long eventId,
        [Description("JSON object with the fields to update.")] string bodyJson,
        [Description("Athlete id. Use '0' for the API key owner.")] string athleteId = "0",
        CancellationToken ct = default)
    {
        var body = JsonHelpers.ParseOptionalBody(bodyJson) ?? throw new ArgumentException("bodyJson is required");
        var result = await client.PutAsync($"/api/v1/athlete/{AthleteTools.Id(client, athleteId)}/events/{eventId}", body, ct: ct);
        return result.ToPrettyJson();
    }

    [McpServerTool(Name = "intervals_delete_event"), Description("Delete a single calendar event/planned workout by id.")]
    public static async Task<string> DeleteEvent(
        IntervalsClient client,
        [Description("Event id.")] long eventId,
        [Description("Athlete id. Use '0' for the API key owner.")] string athleteId = "0",
        CancellationToken ct = default)
    {
        var result = await client.DeleteAsync($"/api/v1/athlete/{AthleteTools.Id(client, athleteId)}/events/{eventId}", ct: ct);
        return result.ToPrettyJson();
    }

    [McpServerTool(Name = "intervals_bulk_create_events"), Description(
        "Create multiple calendar events/planned workouts in one call — the efficient way for a " +
        "coaching agent to push a full week or block of planned training at once. Body is a JSON " +
        "array of Event objects.")]
    public static async Task<string> BulkCreateEvents(
        IntervalsClient client,
        [Description("JSON array of new event objects.")] string bodyJson,
        [Description("Athlete id. Use '0' for the API key owner.")] string athleteId = "0",
        CancellationToken ct = default)
    {
        var body = JsonHelpers.ParseOptionalBody(bodyJson) ?? throw new ArgumentException("bodyJson is required");
        var result = await client.PostAsync($"/api/v1/athlete/{AthleteTools.Id(client, athleteId)}/events/bulk", body, ct: ct);
        return result.ToPrettyJson();
    }
}
