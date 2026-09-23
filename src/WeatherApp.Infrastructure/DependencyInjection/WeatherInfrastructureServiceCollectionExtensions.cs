using Microsoft.Extensions.DependencyInjection;
using WeatherApp.Infrastructure.Weather;

namespace WeatherApp.Infrastructure;

public static class WeatherInfrastructureServiceCollectionExtensions
{
    public const string OpenMeteoHttpClientName = "OpenMeteo";

    public static IServiceCollection AddOpenMeteoWeatherClient(
        this IServiceCollection services,
        Action<HttpClient>? configure = null)
    {
        services.AddHttpClient(OpenMeteoHttpClientName, client => configure?.Invoke(client));

        services.AddSingleton(static sp =>
        {
            HttpClient httpClient = sp.GetRequiredService<IHttpClientFactory>()
                .CreateClient(OpenMeteoHttpClientName);
            return new OpenMeteoGeocoder(httpClient);
        });

        services.AddTransient<IWeatherClient>(static sp =>
        {
            HttpClient httpClient = sp.GetRequiredService<IHttpClientFactory>()
                .CreateClient(OpenMeteoHttpClientName);
            OpenMeteoGeocoder geocoder = sp.GetRequiredService<OpenMeteoGeocoder>();
            return new WeatherClient(httpClient, geocoder);
        });

        return services;
    }
}
