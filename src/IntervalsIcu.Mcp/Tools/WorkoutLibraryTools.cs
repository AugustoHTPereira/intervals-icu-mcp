using System.ComponentModel;
using IntervalsIcu.Client;
using ModelContextProtocol.Server;

namespace IntervalsIcu.Mcp.Tools;

/// <summary>
/// Reusable workout templates stored in the athlete's Intervals.ICU library (as opposed to
/// EventTools, which schedules a workout onto a specific calendar date).
/// </summary>
[McpServerToolType]
public static class WorkoutLibraryTools
{
    [McpServerTool(Name = "intervals_list_workouts"), Description(
        "List all reusable workout templates in the athlete's workout library.")]
    public static async Task<string> ListWorkouts(
        IntervalsClient client,
        [Description("Athlete id. Use '0' for the API key owner.")] string athleteId = "0",
        CancellationToken ct = default)
    {
        var result = await client.GetAsync($"/api/v1/athlete/{AthleteTools.Id(client, athleteId)}/workouts", ct: ct);
        return result.ToPrettyJson();
    }

    [McpServerTool(Name = "intervals_get_workout"), Description("Get a single workout template by id, including its full workout_doc structure.")]
    public static async Task<string> GetWorkout(
        IntervalsClient client,
        [Description("Workout id.")] long workoutId,
        [Description("Athlete id. Use '0' for the API key owner.")] string athleteId = "0",
        CancellationToken ct = default)
    {
        var result = await client.GetAsync($"/api/v1/athlete/{AthleteTools.Id(client, athleteId)}/workouts/{workoutId}", ct: ct);
        return result.ToPrettyJson();
    }

    [McpServerTool(Name = "intervals_create_workout"), Description(
        "Create a reusable workout template in the athlete's library (not scheduled on a date — use " +
        "intervals_create_event to schedule it, or set folder_id here to file it in a plan folder). " +
        "Body is a JSON object with Workout schema fields, including a 'workout_doc' describing steps.")]
    public static async Task<string> CreateWorkout(
        IntervalsClient client,
        [Description("JSON object describing the workout, e.g. {\"name\": \"VO2 5x4\", \"type\": \"Ride\", \"workout_doc\": {...}}")] string bodyJson,
        [Description("Athlete id. Use '0' for the API key owner.")] string athleteId = "0",
        CancellationToken ct = default)
    {
        var body = JsonHelpers.ParseOptionalBody(bodyJson) ?? throw new ArgumentException("bodyJson is required");
        var result = await client.PostAsync($"/api/v1/athlete/{AthleteTools.Id(client, athleteId)}/workouts", body, ct: ct);
        return result.ToPrettyJson();
    }

    [McpServerTool(Name = "intervals_update_workout"), Description("Update an existing workout template by id.")]
    public static async Task<string> UpdateWorkout(
        IntervalsClient client,
        [Description("Workout id.")] long workoutId,
        [Description("JSON object with the fields to update.")] string bodyJson,
        [Description("Athlete id. Use '0' for the API key owner.")] string athleteId = "0",
        CancellationToken ct = default)
    {
        var body = JsonHelpers.ParseOptionalBody(bodyJson) ?? throw new ArgumentException("bodyJson is required");
        var result = await client.PutAsync($"/api/v1/athlete/{AthleteTools.Id(client, athleteId)}/workouts/{workoutId}", body, ct: ct);
        return result.ToPrettyJson();
    }

    [McpServerTool(Name = "intervals_delete_workout"), Description("Delete a workout template from the library by id.")]
    public static async Task<string> DeleteWorkout(
        IntervalsClient client,
        [Description("Workout id.")] long workoutId,
        [Description("Athlete id. Use '0' for the API key owner.")] string athleteId = "0",
        CancellationToken ct = default)
    {
        var result = await client.DeleteAsync($"/api/v1/athlete/{AthleteTools.Id(client, athleteId)}/workouts/{workoutId}", ct: ct);
        return result.ToPrettyJson();
    }

    [McpServerTool(Name = "intervals_list_workout_folders"), Description(
        "List the folders (e.g. training plans, workout categories) in the athlete's workout library.")]
    public static async Task<string> ListFolders(
        IntervalsClient client,
        [Description("Athlete id. Use '0' for the API key owner.")] string athleteId = "0",
        CancellationToken ct = default)
    {
        var result = await client.GetAsync($"/api/v1/athlete/{AthleteTools.Id(client, athleteId)}/folders", ct: ct);
        return result.ToPrettyJson();
    }
}
