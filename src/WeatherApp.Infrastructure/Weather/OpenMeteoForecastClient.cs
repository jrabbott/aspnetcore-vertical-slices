using System.Globalization;
using System.Net.Http.Json;
using WeatherApp.Domain.Weather;

namespace WeatherApp.Infrastructure.Weather;

internal sealed class OpenMeteoForecastClient(HttpClient httpClient)
{
    private const string ForecastBase = "https://api.open-meteo.com/v1/forecast";

    private readonly HttpClient _httpClient = httpClient;

    public async Task<WeatherReading?> GetCurrentAsync(
        GeoLocation location,
        CancellationToken cancellationToken)
    {
        string url =
            $"{ForecastBase}?latitude={Format(location.Latitude)}&longitude={Format(location.Longitude)}"
            + "&current=temperature_2m,relative_humidity_2m,weather_code,wind_speed_10m"
            + "&timezone=auto&wind_speed_unit=kmh";

        ForecastResponse? response = await _httpClient
            .GetFromJsonAsync<ForecastResponse>(url, cancellationToken)
            .ConfigureAwait(false);

        return response?.Current is null ? null : MapCurrent(location, response.Current);
    }

    public async Task<IReadOnlyList<WeatherReading>> GetDailyAsync(
        GeoLocation location,
        int days,
        CancellationToken cancellationToken)
    {
        string url =
            $"{ForecastBase}?latitude={Format(location.Latitude)}&longitude={Format(location.Longitude)}"
            + "&daily=weather_code,temperature_2m_max,relative_humidity_2m_mean,wind_speed_10m_max"
            + $"&forecast_days={days}&timezone=auto&wind_speed_unit=kmh";

        ForecastResponse? response = await _httpClient
            .GetFromJsonAsync<ForecastResponse>(url, cancellationToken)
            .ConfigureAwait(false);

        return MapDaily(location, response?.Daily);
    }

    private static WeatherReading MapCurrent(GeoLocation location, CurrentWeather current)
    {
        return new WeatherReading
        {
            Location = new Location { City = location.City, Country = location.Country },
            Date = ParseDate(current.Time) ?? DateOnly.FromDateTime(DateTime.UtcNow),
            TemperatureC = (int)Math.Round(current.Temperature2M),
            Summary = WeatherCodeMapper.ToSummary(current.WeatherCode),
            HumidityPercent = current.RelativeHumidity2M,
            WindSpeedKph = (int)Math.Round(current.WindSpeed10M)
        };
    }

    private static List<WeatherReading> MapDaily(GeoLocation location, DailyWeather? daily)
    {
        if (daily?.Time is null || daily.Time.Count == 0)
        {
            return [];
        }

        var readings = new List<WeatherReading>(daily.Time.Count);

        for (int i = 0; i < daily.Time.Count; i++)
        {
            readings.Add(new WeatherReading
            {
                Location = new Location { City = location.City, Country = location.Country },
                Date = DateOnly.Parse(daily.Time[i], CultureInfo.InvariantCulture),
                TemperatureC = (int)Math.Round(ValueAt(daily.Temperature2MMax, i)),
                Summary = WeatherCodeMapper.ToSummary((int)ValueAt(daily.WeatherCode, i)),
                HumidityPercent = (int)Math.Round(ValueAt(daily.RelativeHumidity2MMean, i)),
                WindSpeedKph = (int)Math.Round(ValueAt(daily.WindSpeed10MMax, i))
            });
        }

        return readings;
    }

    private static string Format(double value)
    {
        return value.ToString("0.####", CultureInfo.InvariantCulture);
    }

    private static DateOnly? ParseDate(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime parsed)
            ? DateOnly.FromDateTime(parsed)
            : null;
    }

    private static double ValueAt(List<double>? values, int index)
    {
        return values is not null && index < values.Count ? values[index] : 0;
    }
}
