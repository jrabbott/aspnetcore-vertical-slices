using WeatherApp.Domain.Weather;

namespace WeatherApp.Features.Weather.Search;

public sealed class SearchResponse
{
    public SearchRequest Request { get; init; } = new();
    public bool Searched { get; init; }
    public bool Found { get; init; }
    public string? ErrorMessage { get; init; }
    public string? City { get; init; }
    public string? Country { get; init; }
    public int? TemperatureC { get; init; }
    public int? TemperatureF { get; init; }
    public string? Summary { get; init; }
    public int? HumidityPercent { get; init; }
    public int? WindSpeedKph { get; init; }
    public IReadOnlyList<string> ExampleCities { get; init; } = [];

    public static SearchResponse Empty(SearchRequest request, IReadOnlyList<string> exampleCities) =>
        new()
        {
            Request = request,
            Searched = false,
            ExampleCities = exampleCities
        };

    public static SearchResponse Invalid(
        SearchRequest request,
        string errorMessage,
        IReadOnlyList<string> exampleCities) =>
        new()
        {
            Request = request,
            Searched = true,
            Found = false,
            ErrorMessage = errorMessage,
            ExampleCities = exampleCities
        };

    public static SearchResponse NotFound(
        SearchRequest request,
        string city,
        IReadOnlyList<string> exampleCities) =>
        Invalid(
            request,
            $"No weather data found for \"{city.Trim()}\". Try one of the example cities.",
            exampleCities);

    public static SearchResponse FromReading(
        SearchRequest request,
        WeatherReading reading,
        IReadOnlyList<string> exampleCities) =>
        new()
        {
            Request = request,
            Searched = true,
            Found = true,
            City = reading.Location.City,
            Country = reading.Location.Country,
            TemperatureC = reading.TemperatureC,
            TemperatureF = reading.TemperatureF,
            Summary = reading.Summary,
            HumidityPercent = reading.HumidityPercent,
            WindSpeedKph = reading.WindSpeedKph,
            ExampleCities = exampleCities
        };
}
