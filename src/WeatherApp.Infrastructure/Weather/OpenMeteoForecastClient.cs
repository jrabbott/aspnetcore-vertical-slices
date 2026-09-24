using System.Globalization;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using WeatherApp.Domain.Weather;

namespace WeatherApp.Infrastructure.Weather;

internal sealed class OpenMeteoForecastClient
{
    private readonly HttpClient _httpClient;
    private readonly OpenMeteoOptions _options;

    public OpenMeteoForecastClient(HttpClient httpClient, IOptions<OpenMeteoOptions> options)
        : this(httpClient, options?.Value ?? throw new ArgumentNullException(nameof(options)))
    {
    }

    internal OpenMeteoForecastClient(HttpClient httpClient, OpenMeteoOptions options)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(options);
        _httpClient = httpClient;
        _options = options;
    }

    public async Task<WeatherReading?> GetCurrentAsync(
        GeoLocation location,
        CancellationToken cancellationToken)
    {
        string url =
            $"{_options.ForecastBaseUrl.TrimEnd('/')}?latitude={Format(location.Latitude)}&longitude={Format(location.Longitude)}"
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
            $"{_options.ForecastBaseUrl.TrimEnd('/')}?latitude={Format(location.Latitude)}&longitude={Format(location.Longitude)}"
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
            if (!TryGetValue(daily.Temperature2MMax, i, out double temperature)
                || !TryGetValue(daily.WeatherCode, i, out double weatherCode)
                || !TryGetValue(daily.RelativeHumidity2MMean, i, out double humidity)
                || !TryGetValue(daily.WindSpeed10MMax, i, out double windSpeed)
                || !DateOnly.TryParse(daily.Time[i], CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly date))
            {
                continue;
            }

            readings.Add(new WeatherReading
            {
                Location = new Location { City = location.City, Country = location.Country },
                Date = date,
                TemperatureC = (int)Math.Round(temperature),
                Summary = WeatherCodeMapper.ToSummary((int)weatherCode),
                HumidityPercent = (int)Math.Round(humidity),
                WindSpeedKph = (int)Math.Round(windSpeed)
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

    private static bool TryGetValue(List<double>? values, int index, out double value)
    {
        if (values is not null && index < values.Count)
        {
            value = values[index];
            return true;
        }

        value = 0;
        return false;
    }
}
