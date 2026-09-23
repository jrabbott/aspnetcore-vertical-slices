using WeatherApp.Domain.Weather;
using WeatherApp.Infrastructure.Favourites;
using WeatherApp.Infrastructure.Weather;

namespace WeatherApp.Features.Weather.Favourites;

public sealed class FavouritesHandler(IFavouritesStore favouritesStore, IWeatherClient weatherClient)
{
    private readonly IFavouritesStore _favouritesStore = favouritesStore;
    private readonly IWeatherClient _weatherClient = weatherClient;

    public async Task<FavouritesResponse> HandleAsync(
        FavouritesRequest request,
        string? statusMessage = null,
        bool statusIsError = false,
        CancellationToken cancellationToken = default)
    {
        _ = request;

        IReadOnlyList<string> favouriteCities = _favouritesStore.GetAll();
        IReadOnlyList<FavouriteCity> items = await LoadFavouriteCitiesAsync(favouriteCities, cancellationToken);
        IReadOnlyList<string> suggested = BuildSuggestedCities(favouriteCities);

        return FavouritesResponse.Create(items, suggested, statusMessage, statusIsError);
    }

    private async Task<IReadOnlyList<FavouriteCity>> LoadFavouriteCitiesAsync(
        IReadOnlyList<string> favouriteCities,
        CancellationToken cancellationToken)
    {
        FavouriteCity[] items = await Task.WhenAll(
            favouriteCities.Select(async city =>
            {
                WeatherReading? reading = await _weatherClient.GetCurrentAsync(city, cancellationToken);
                return FavouriteCity.FromReading(city, reading);
            }));

        return items;
    }

    private static IReadOnlyList<string> BuildSuggestedCities(IReadOnlyList<string> favouriteCities)
    {
        return
        [
            .. WeatherClient.ExampleCities
                .Where(c => !favouriteCities.Contains(c, StringComparer.OrdinalIgnoreCase))
        ];
    }
}
