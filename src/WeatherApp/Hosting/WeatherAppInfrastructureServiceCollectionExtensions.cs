using WeatherApp.Infrastructure.Favorites;
using WeatherApp.Infrastructure.Weather;

namespace WeatherApp.Hosting;

internal static class WeatherAppInfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddWeatherAppInfrastructure(this IServiceCollection services)
    {
        services.AddHttpClient<IWeatherClient, WeatherClient>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(15);
            client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "WeatherApp/1.0 (+https://github.com/jrabbott/aspnetcore-vertical-slices; Open-Meteo)");
        });

        services.AddSingleton<IFavoritesStore, FavoritesStore>();
        return services;
    }
}
