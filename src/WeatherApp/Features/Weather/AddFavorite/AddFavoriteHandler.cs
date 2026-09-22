using WeatherApp.Infrastructure.Favorites;
using WeatherApp.Infrastructure.Weather;

namespace WeatherApp.Features.Weather.AddFavorite;

public sealed class AddFavoriteHandler
{
    private readonly IFavoritesStore _favoritesStore;
    private readonly IWeatherClient _weatherClient;

    public AddFavoriteHandler(IFavoritesStore favoritesStore, IWeatherClient weatherClient)
    {
        _favoritesStore = favoritesStore;
        _weatherClient = weatherClient;
    }

    public async Task<AddFavoriteResult> HandleAsync(
        AddFavoriteRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.City))
        {
            return AddFavoriteResult.Fail("Please enter a city name.");
        }

        var city = request.City.Trim();
        var reading = await _weatherClient.GetCurrentAsync(city, cancellationToken);

        if (reading is null)
        {
            return AddFavoriteResult.Fail($"\"{city}\" is not a supported city.");
        }

        var added = _favoritesStore.Add(reading.Location.City);
        if (!added)
        {
            return AddFavoriteResult.Fail($"{reading.Location.City} is already in your favorites.");
        }

        return AddFavoriteResult.Ok($"{reading.Location.City} was added to your favorites.");
    }
}

public sealed class AddFavoriteResult
{
    public required bool Succeeded { get; init; }
    public required string Message { get; init; }

    public static AddFavoriteResult Ok(string message) => new() { Succeeded = true, Message = message };
    public static AddFavoriteResult Fail(string message) => new() { Succeeded = false, Message = message };
}
