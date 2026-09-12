using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IntervalsIcu.Client;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="IntervalsClient"/> and its <see cref="HttpClient"/>, bound from the
    /// "Intervals" configuration section (populated from env vars INTERVALS_API_KEY,
    /// INTERVALS_BASE_URL, INTERVALS_DEFAULT_ATHLETE_ID).
    /// </summary>
    public static IServiceCollection AddIntervalsIcuClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<IntervalsClientOptions>(configuration.GetSection(IntervalsClientOptions.SectionName));
        services.AddHttpClient<IntervalsClient>();
        return services;
    }
}
