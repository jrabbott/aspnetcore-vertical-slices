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
}
