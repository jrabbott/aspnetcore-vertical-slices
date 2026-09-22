namespace WeatherApp.Domain.Weather;

public sealed class WeatherReading
{
    public required Location Location { get; init; }
    public required DateOnly Date { get; init; }
    public required int TemperatureC { get; init; }
    public required string Summary { get; init; }
    public required int HumidityPercent { get; init; }
    public required int WindSpeedKph { get; init; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
