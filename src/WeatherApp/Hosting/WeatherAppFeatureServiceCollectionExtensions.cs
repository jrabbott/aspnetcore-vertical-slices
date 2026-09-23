using FluentValidation;
using WeatherApp.Features.Weather.AddFavourite;
using WeatherApp.Features.Weather.Favourites;
using WeatherApp.Features.Weather.Forecast;
using WeatherApp.Features.Weather.RemoveFavourite;
using WeatherApp.Features.Weather.Search;

namespace WeatherApp.Hosting;

internal static class WeatherAppFeatureServiceCollectionExtensions
{
    public static IServiceCollection AddWeatherAppFeatures(this IServiceCollection services)
    {
        services.AddTransient<IValidator<SearchRequest>, SearchRequestValidator>();
        services.AddTransient<IValidator<ForecastRequest>, ForecastRequestValidator>();
        services.AddTransient<IValidator<AddFavouriteRequest>, AddFavouriteRequestValidator>();
        services.AddTransient<IValidator<RemoveFavouriteRequest>, RemoveFavouriteRequestValidator>();

        services.AddTransient<SearchHandler>();
        services.AddTransient<ForecastHandler>();
        services.AddTransient<FavouritesHandler>();
        services.AddTransient<AddFavouriteHandler>();
        services.AddTransient<RemoveFavouriteHandler>();

        return services;
    }
}
