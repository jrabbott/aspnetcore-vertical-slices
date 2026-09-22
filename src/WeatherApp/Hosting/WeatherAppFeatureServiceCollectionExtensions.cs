using FluentValidation;
using WeatherApp.Features.Weather.AddFavorite;
using WeatherApp.Features.Weather.Favorites;
using WeatherApp.Features.Weather.Forecast;
using WeatherApp.Features.Weather.RemoveFavorite;
using WeatherApp.Features.Weather.Search;

namespace WeatherApp.Hosting;

internal static class WeatherAppFeatureServiceCollectionExtensions
{
    public static IServiceCollection AddWeatherAppFeatures(this IServiceCollection services)
    {
        services.AddTransient<IValidator<SearchRequest>, SearchRequestValidator>();
        services.AddTransient<IValidator<ForecastRequest>, ForecastRequestValidator>();
        services.AddTransient<IValidator<AddFavoriteRequest>, AddFavoriteRequestValidator>();
        services.AddTransient<IValidator<RemoveFavoriteRequest>, RemoveFavoriteRequestValidator>();

        services.AddTransient<SearchHandler>();
        services.AddTransient<ForecastHandler>();
        services.AddTransient<FavoritesHandler>();
        services.AddTransient<AddFavoriteHandler>();
        services.AddTransient<RemoveFavoriteHandler>();

        return services;
    }
}
