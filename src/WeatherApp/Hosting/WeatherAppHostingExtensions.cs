namespace WeatherApp.Hosting;

internal static class WeatherAppHostingExtensions
{
    public static IServiceCollection AddWeatherApp(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddWeatherAppPlatform();
        services.AddWeatherAppMvc();
        services.AddWeatherAppInfrastructure(configuration);
        services.AddWeatherAppFeatures();
        return services;
    }

    public static WebApplication UseWeatherApp(this WebApplication app)
    {
        app.UseWeatherAppExceptionHandling();
        app.UseWeatherAppRequestPipeline();
        app.MapWeatherAppEndpoints();
        return app;
    }
}
