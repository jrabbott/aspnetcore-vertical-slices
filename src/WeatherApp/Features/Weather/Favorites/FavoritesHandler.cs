using WeatherApp.Infrastructure.Favorites;
using WeatherApp.Infrastructure.Weather;

namespace WeatherApp.Features.Weather.Favorites;

public sealed class FavoritesHandler
{
    private readonly IFavoritesStore _favoritesStore;
    private readonly IWeatherClient _weatherClient;

    public FavoritesHandler(IFavoritesStore favoritesStore, IWeatherClient weatherClient)
    {
        _favoritesStore = favoritesStore;
        _weatherClient = weatherClient;
    }

    public async Task<FavoritesResponse> HandleAsync(
        FavoritesRequest request,
        string? statusMessage = null,
        bool statusIsError = false,
        CancellationToken cancellationToken = default)
    {
        _ = request;

        var favoriteCities = _favoritesStore.GetAll();
        var items = new List<FavoriteCity>(favoriteCities.Count);

        foreach (var city in favoriteCities)
        {
            var reading = await _weatherClient.GetCurrentAsync(city, cancellationToken);
            items.Add(FavoriteCity.FromReading(city, reading));
        }

        var suggested = WeatherClient.KnownCities
            .Where(c => !favoriteCities.Contains(c, StringComparer.OrdinalIgnoreCase))
            .ToArray();

        return FavoritesResponse.Create(items, suggested, statusMessage, statusIsError);
    }
}
