using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using WeatherApp.Razor;

namespace WeatherApp.Hosting;

internal static class WeatherAppMvcServiceCollectionExtensions
{
    public static IServiceCollection AddWeatherAppMvc(this IServiceCollection services)
    {
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            // Azure Container Apps (and similar ingress) terminate TLS and forward the original scheme.
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });
        services.AddHealthChecks();
        services.AddControllersWithViews(options =>
            options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));
        services.Configure<RazorViewEngineOptions>(options =>
            options.ViewLocationExpanders.Add(new FeatureViewLocationExpander()));
        return services;
    }
}
