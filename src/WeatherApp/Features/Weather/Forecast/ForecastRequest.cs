namespace WeatherApp.Features.Weather.Forecast;

public sealed class ForecastRequest
{
    public string? City { get; set; }

    public int Days { get; set; } = 5;
}
