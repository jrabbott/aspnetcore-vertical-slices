namespace WeatherApp.Features.Weather.Forecast;

public sealed class ForecastResponse
{
    public ForecastRequest Request { get; init; } = new();
    public bool Searched { get; init; }
    public bool Found { get; init; }
    public string? ErrorMessage { get; init; }
    public string? City { get; init; }
    public string? Country { get; init; }
    public IReadOnlyList<ForecastDay> Days { get; init; } = [];
    public IReadOnlyList<string> ExampleCities { get; init; } = [];
}

public sealed class ForecastDay
{
    public required DateOnly Date { get; init; }
    public required int TemperatureC { get; init; }
    public required int TemperatureF { get; init; }
    public required string Summary { get; init; }
    public required int HumidityPercent { get; init; }
    public required int WindSpeedKph { get; init; }
}
