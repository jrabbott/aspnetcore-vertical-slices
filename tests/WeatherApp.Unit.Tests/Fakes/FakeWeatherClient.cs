using WeatherApp.Domain.Weather;
using WeatherApp.Infrastructure.Weather;

namespace WeatherApp.Unit.Tests.Fakes;

internal sealed class FakeWeatherClient : IWeatherClient
{
    private readonly Dictionary<string, WeatherReading> _readings;

    public FakeWeatherClient(params WeatherReading[] readings)
    {
        _readings = readings.ToDictionary(
            r => r.Location.City,
            r => r,
            StringComparer.OrdinalIgnoreCase);
    }

    public Task<WeatherReading?> GetCurrentAsync(string city, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(city) || !_readings.TryGetValue(city.Trim(), out var reading))
        {
            return Task.FromResult<WeatherReading?>(null);
        }

        return Task.FromResult<WeatherReading?>(reading);
    }

    public Task<IReadOnlyList<WeatherReading>> GetForecastAsync(
        string city,
        int days,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(city) || !_readings.TryGetValue(city.Trim(), out var reading))
        {
            return Task.FromResult<IReadOnlyList<WeatherReading>>(Array.Empty<WeatherReading>());
        }

        days = Math.Clamp(days, 1, 7);
        var forecast = Enumerable.Range(0, days)
            .Select(offset => new WeatherReading
            {
                Location = reading.Location,
                Date = reading.Date.AddDays(offset),
                TemperatureC = reading.TemperatureC + offset,
                Summary = reading.Summary,
                HumidityPercent = reading.HumidityPercent,
                WindSpeedKph = reading.WindSpeedKph
            })
            .ToArray();

        return Task.FromResult<IReadOnlyList<WeatherReading>>(forecast);
    }

    public static WeatherReading Reading(
        string city,
        string country = "Testland",
        int temperatureC = 20,
        string summary = "Clear") =>
        new()
        {
            Location = new Location { City = city, Country = country },
            Date = new DateOnly(2026, 9, 22),
            TemperatureC = temperatureC,
            Summary = summary,
            HumidityPercent = 50,
            WindSpeedKph = 10
        };
}
