using WeatherApp.Domain.Weather;

namespace WeatherApp.Features.Weather.Favorites;

public sealed class FavoritesResponse
{
    public IReadOnlyList<FavoriteCity> Cities { get; init; } = [];
    public IReadOnlyList<string> SuggestedCities { get; init; } = [];
    public string? StatusMessage { get; init; }
    public bool StatusIsError { get; init; }

    public static FavoritesResponse Create(
        IReadOnlyList<FavoriteCity> cities,
        IReadOnlyList<string> suggestedCities,
        string? statusMessage = null,
        bool statusIsError = false) =>
        new()
        {
            Cities = cities,
            SuggestedCities = suggestedCities,
            StatusMessage = statusMessage,
            StatusIsError = statusIsError
        };
}

public sealed class FavoriteCity
{
    public required string City { get; init; }
    public string? Country { get; init; }
    public int? TemperatureC { get; init; }
    public string? Summary { get; init; }
    public bool HasWeather { get; init; }

    public static FavoriteCity FromReading(string city, WeatherReading? reading) =>
        new()
        {
            City = city,
            Country = reading?.Location.Country,
            TemperatureC = reading?.TemperatureC,
            Summary = reading?.Summary,
            HasWeather = reading is not null
        };
}
