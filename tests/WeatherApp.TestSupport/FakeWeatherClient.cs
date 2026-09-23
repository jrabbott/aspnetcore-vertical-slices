using WeatherApp.Domain.Weather;
using WeatherApp.Infrastructure.Weather;

namespace WeatherApp.TestSupport;

/// <summary>
/// In-memory <see cref="IWeatherClient"/> for unit and integration tests.
/// Production uses Open-Meteo-backed <c>WeatherClient</c> in Infrastructure.
/// </summary>
public sealed class FakeWeatherClient(params WeatherReading[] readings) : IWeatherClient
{
    private readonly Dictionary<string, WeatherReading> _readings = readings.ToDictionary(
            r => r.Location.City,
            r => r,
            StringComparer.OrdinalIgnoreCase);

    public Task<WeatherReading?> GetCurrentAsync(string city, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return string.IsNullOrWhiteSpace(city) || !_readings.TryGetValue(city.Trim(), out WeatherReading? reading)
            ? Task.FromResult<WeatherReading?>(null)
            : Task.FromResult<WeatherReading?>(reading);
    }

    public Task<IReadOnlyList<WeatherReading>> GetForecastAsync(
        string city,
        int days,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(city) || !_readings.TryGetValue(city.Trim(), out WeatherReading? reading))
        {
            return Task.FromResult<IReadOnlyList<WeatherReading>>(Array.Empty<WeatherReading>());
        }

        days = Math.Clamp(days, 1, 7);
        WeatherReading[] forecast = [.. Enumerable.Range(0, days)
            .Select(offset => new WeatherReading
            {
                Location = reading.Location,
                Date = reading.Date.AddDays(offset),
                TemperatureC = reading.TemperatureC + offset,
                Summary = reading.Summary,
                HumidityPercent = reading.HumidityPercent,
                WindSpeedKph = reading.WindSpeedKph
            })];

        return Task.FromResult<IReadOnlyList<WeatherReading>>(forecast);
    }

    public static WeatherReading Reading(
        string city,
        string country = "Testland",
        int temperatureC = 20,
        string summary = "Clear")
    {
        return new()
        {
            Location = new Location { City = city, Country = country },
            Date = new DateOnly(2026, 9, 22),
            TemperatureC = temperatureC,
            Summary = summary,
            HumidityPercent = 50,
            WindSpeedKph = 10
        };
    }
}
