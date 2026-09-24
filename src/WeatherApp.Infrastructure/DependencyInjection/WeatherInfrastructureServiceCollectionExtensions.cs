using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WeatherApp.Infrastructure.Weather;

namespace WeatherApp.Infrastructure;

public static class WeatherInfrastructureServiceCollectionExtensions
{
    public const string OpenMeteoHttpClientName = "OpenMeteo";

    public static IServiceCollection AddOpenMeteoWeatherClient(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<HttpClient>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        services.Configure<OpenMeteoOptions>(configuration.GetSection(OpenMeteoOptions.SectionName));
        services.AddHttpClient(OpenMeteoHttpClientName, client => configure?.Invoke(client));
        RegisterOpenMeteoServices(services);
        return services;
    }

    private static void RegisterOpenMeteoServices(IServiceCollection services)
    {
        services.AddSingleton(CreateGeocoder);
        services.AddTransient<IWeatherClient>(CreateWeatherClient);
    }

    private static OpenMeteoGeocoder CreateGeocoder(IServiceProvider sp)
    {
        return new OpenMeteoGeocoder(CreateNamedHttpClient(sp), GetOptions(sp));
    }

    private static IWeatherClient CreateWeatherClient(IServiceProvider sp)
    {
        return new WeatherClient(CreateNamedHttpClient(sp), sp.GetRequiredService<OpenMeteoGeocoder>(), GetOptions(sp).Value);
    }

    private static HttpClient CreateNamedHttpClient(IServiceProvider sp)
    {
        return sp.GetRequiredService<IHttpClientFactory>().CreateClient(OpenMeteoHttpClientName);
    }

    private static IOptions<OpenMeteoOptions> GetOptions(IServiceProvider sp)
    {
        return sp.GetRequiredService<IOptions<OpenMeteoOptions>>();
    }
}
