using Microsoft.AspNetCore.HttpOverrides;

namespace WeatherApp.Hosting;

internal static class WeatherAppPlatformServiceCollectionExtensions
{
    public static IServiceCollection AddWeatherAppPlatform(this IServiceCollection services)
    {
        services.Configure<ForwardedHeadersOptions>(ConfigureForwardedHeaders);
        services.AddHealthChecks();
        return services;
    }

    private static void ConfigureForwardedHeaders(ForwardedHeadersOptions options)
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        // Azure Container Apps (and similar ingress) terminate TLS and forward the original scheme.
        options.KnownIPNetworks.Clear();
        options.KnownProxies.Clear();
    }
}
