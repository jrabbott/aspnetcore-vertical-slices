using WeatherApp.Domain.Weather;

namespace WeatherApp.Features.Weather.Favourites;

public sealed class FavouritesResponse
{
    public IReadOnlyList<FavouriteCity> Cities { get; init; } = [];
    public IReadOnlyList<string> SuggestedCities { get; init; } = [];
    public string? StatusMessage
    {
        get; init;
    }
    public bool StatusIsError
    {
        get; init;
    }

    public static FavouritesResponse Create(
        IReadOnlyList<FavouriteCity> cities,
        IReadOnlyList<string> suggestedCities,
        string? statusMessage = null,
        bool statusIsError = false)
    {
        return new()
        {
            Cities = cities,
            SuggestedCities = suggestedCities,
            StatusMessage = statusMessage,
            StatusIsError = statusIsError
        };
    }
}

public sealed class FavouriteCity
{
    public required string City
    {
        get; init;
    }
    public string? Country
    {
        get; init;
    }
    public int? TemperatureC
    {
        get; init;
    }
    public string? Summary
    {
        get; init;
    }
    public bool HasWeather
    {
        get; init;
    }

    public static FavouriteCity FromReading(string city, WeatherReading? reading)
    {
        return new()
        {
            City = city,
            Country = reading?.Location.Country,
            TemperatureC = reading?.TemperatureC,
            Summary = reading?.Summary,
            HasWeather = reading is not null
        };
    }
}
