using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using WeatherApp.Razor;

namespace WeatherApp.Hosting;

internal static class WeatherAppMvcServiceCollectionExtensions
{
    public static IServiceCollection AddWeatherAppMvc(this IServiceCollection services)
    {
        services.AddControllersWithViews(options =>
            options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));
        services.Configure<RazorViewEngineOptions>(options =>
            options.ViewLocationExpanders.Add(new FeatureViewLocationExpander()));
        return services;
    }
}
