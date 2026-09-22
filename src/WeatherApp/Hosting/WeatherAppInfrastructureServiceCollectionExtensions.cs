using WeatherApp.Infrastructure.Favorites;
using WeatherApp.Infrastructure.Weather;

namespace WeatherApp.Hosting;

internal static class WeatherAppInfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddWeatherAppInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IWeatherClient, WeatherClient>();
        services.AddSingleton<IFavoritesStore, FavoritesStore>();
        return services;
    }
}
