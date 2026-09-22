using WeatherApp.Domain.Weather;
using WeatherApp.Infrastructure.Favorites;
using WeatherApp.Infrastructure.Weather;

namespace WeatherApp.Features.Weather.Favorites;

public sealed class FavoritesHandler(IFavoritesStore favoritesStore, IWeatherClient weatherClient)
{
    private readonly IFavoritesStore _favoritesStore = favoritesStore;
    private readonly IWeatherClient _weatherClient = weatherClient;

    public async Task<FavoritesResponse> HandleAsync(
        FavoritesRequest request,
        string? statusMessage = null,
        bool statusIsError = false,
        CancellationToken cancellationToken = default)
    {
        _ = request;

        IReadOnlyList<string> favoriteCities = _favoritesStore.GetAll();
        var items = new List<FavoriteCity>(favoriteCities.Count);

        foreach (string city in favoriteCities)
        {
            WeatherReading? reading = await _weatherClient.GetCurrentAsync(city, cancellationToken);
            items.Add(FavoriteCity.FromReading(city, reading));
        }

        string[] suggested =
        [
            .. WeatherClient.KnownCities
                .Where(c => !favoriteCities.Contains(c, StringComparer.OrdinalIgnoreCase))
        ];

        return FavoritesResponse.Create(items, suggested, statusMessage, statusIsError);
    }
}
