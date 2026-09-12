using IntervalsIcu.Client;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    options.ListenAnyIP(int.Parse(port));
});

builder.Services.AddIntervalsIcuClient(builder.Configuration);
builder.Services.PostConfigure<IntervalsClientOptions>(options =>
{
    options.ApiKey = Environment.GetEnvironmentVariable("INTERVALS_API_KEY") ?? options.ApiKey;
    options.BaseUrl = Environment.GetEnvironmentVariable("INTERVALS_BASE_URL") ?? options.BaseUrl;
    options.DefaultAthleteId = Environment.GetEnvironmentVariable("INTERVALS_DEFAULT_ATHLETE_ID") ?? options.DefaultAthleteId;
});

builder.Services
    .AddMcpServer(options =>
    {
        options.ServerInfo = new() { Name = "intervals-icu-mcp", Version = "1.0.0" };
    })
    .WithHttpTransport()
    .WithToolsFromAssembly();

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new { status = "ok", service = "intervals-icu-mcp" }));
app.MapMcp("/mcp");

app.Run();
