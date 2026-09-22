using WeatherApp.Domain.Weather;

namespace WeatherApp.Infrastructure.Weather;

/// <summary>
/// Deterministic in-memory weather provider. No external API key required.
/// </summary>
public sealed class WeatherClient : IWeatherClient
{
    private static readonly Dictionary<string, CityWeatherProfile> _cities =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["London"] = new("London", "United Kingdom", 12, "Cloudy", 78, 18),
            ["Paris"] = new("Paris", "France", 16, "Partly cloudy", 65, 14),
            ["Madrid"] = new("Madrid", "Spain", 24, "Sunny", 40, 12),
            ["New York"] = new("New York", "United States", 18, "Clear", 55, 20),
            ["Tokyo"] = new("Tokyo", "Japan", 22, "Humid", 70, 10),
        };

    private static readonly string[] _forecastSummaries =
    [
        "Sunny",
        "Partly cloudy",
        "Cloudy",
        "Light rain",
        "Clear",
        "Windy",
        "Overcast"
    ];

    public Task<WeatherReading?> GetCurrentAsync(string city, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!TryResolveCity(city, out CityWeatherProfile profile))
        {
            return Task.FromResult<WeatherReading?>(null);
        }

        var reading = new WeatherReading
        {
            Location = new Location { City = profile.City, Country = profile.Country },
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            TemperatureC = profile.BaseTemperatureC,
            Summary = profile.Summary,
            HumidityPercent = profile.HumidityPercent,
            WindSpeedKph = profile.WindSpeedKph
        };

        return Task.FromResult<WeatherReading?>(reading);
    }

    public Task<IReadOnlyList<WeatherReading>> GetForecastAsync(
        string city,
        int days,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!TryResolveCity(city, out CityWeatherProfile profile))
        {
            return Task.FromResult<IReadOnlyList<WeatherReading>>(Array.Empty<WeatherReading>());
        }

        days = Math.Clamp(days, 1, 7);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        int seed = StableHash(profile.City);
        var readings = new List<WeatherReading>(days);

        for (int i = 0; i < days; i++)
        {
            int temperatureOffset = ((seed + i * 3) % 7) - 3;
            int summaryIndex = Math.Abs(seed + i) % _forecastSummaries.Length;

            readings.Add(new WeatherReading
            {
                Location = new Location { City = profile.City, Country = profile.Country },
                Date = today.AddDays(i),
                TemperatureC = profile.BaseTemperatureC + temperatureOffset,
                Summary = _forecastSummaries[summaryIndex],
                HumidityPercent = Math.Clamp(profile.HumidityPercent + ((seed + i) % 11) - 5, 20, 95),
                WindSpeedKph = Math.Clamp(profile.WindSpeedKph + ((seed + i * 2) % 9) - 4, 5, 40)
            });
        }

        return Task.FromResult<IReadOnlyList<WeatherReading>>(readings);
    }

    public static IReadOnlyCollection<string> KnownCities => _cities.Keys.OrderBy(c => c).ToArray();

    private static bool TryResolveCity(string city, out CityWeatherProfile profile)
    {
        profile = default;

        return !string.IsNullOrWhiteSpace(city) && _cities.TryGetValue(city.Trim(), out profile);
    }

    private static int StableHash(string value)
    {
        unchecked
        {
            int hash = 17;
            foreach (char ch in value.ToUpperInvariant())
            {
                hash = hash * 31 + ch;
            }

            return Math.Abs(hash);
        }
    }

    private readonly record struct CityWeatherProfile(
        string City,
        string Country,
        int BaseTemperatureC,
        string Summary,
        int HumidityPercent,
        int WindSpeedKph);
}
