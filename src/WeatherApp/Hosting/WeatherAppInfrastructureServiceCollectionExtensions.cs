using WeatherApp.Favorites;
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

        services.AddHttpContextAccessor();
        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.Cookie.Name = ".WeatherApp.Session";
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.IdleTimeout = TimeSpan.FromHours(8);
        });

        services.AddScoped<IFavoritesStore, SessionFavoritesStore>();
        return services;
    }
}
