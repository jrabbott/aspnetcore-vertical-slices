namespace WeatherApp.Features.Weather.Favorites;

public sealed class FavoritesResponse
{
    public IReadOnlyList<FavoriteCity> Cities { get; init; } = [];
    public IReadOnlyList<string> SuggestedCities { get; init; } = [];
    public string? StatusMessage { get; init; }
    public bool StatusIsError { get; init; }
}

public sealed class FavoriteCity
{
    public required string City { get; init; }
    public string? Country { get; init; }
    public int? TemperatureC { get; init; }
    public string? Summary { get; init; }
    public bool HasWeather { get; init; }
}
