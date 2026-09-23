using WeatherApp.Favourites;
using WeatherApp.Infrastructure;
using WeatherApp.Infrastructure.Favourites;

namespace WeatherApp.Hosting;

internal static class WeatherAppInfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddWeatherAppInfrastructure(this IServiceCollection services)
    {
        services.AddOpenMeteoWeatherClient(ConfigureOpenMeteoClient);
        services.AddHttpContextAccessor();
        services.AddDistributedMemoryCache();
        services.AddSession(ConfigureSession);
        services.AddScoped<IFavouritesStore, SessionFavouritesStore>();
        return services;
    }

    private static void ConfigureOpenMeteoClient(HttpClient client)
    {
        client.Timeout = TimeSpan.FromSeconds(15);
        client.DefaultRequestHeaders.UserAgent.ParseAdd(
            "WeatherApp/1.0 (+https://github.com/jrabbott/aspnetcore-vertical-slices; Open-Meteo)");
    }

    private static void ConfigureSession(SessionOptions options)
    {
        options.Cookie.Name = ".WeatherApp.Session";
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.IdleTimeout = TimeSpan.FromHours(8);
    }
}
