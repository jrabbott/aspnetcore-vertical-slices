using WeatherApp.Domain.Weather;

namespace WeatherApp.Infrastructure.Weather;

/// <summary>
/// Weather provider backed by the free Open-Meteo geocoding + forecast APIs.
/// No API key is required for non-commercial use.
/// </summary>
public sealed class WeatherClient : IWeatherClient
{
    private readonly OpenMeteoGeocoder _geocoder;
    private readonly OpenMeteoForecastClient _forecast;

    public WeatherClient(HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        _geocoder = new OpenMeteoGeocoder(httpClient);
        _forecast = new OpenMeteoForecastClient(httpClient);
    }

    /// <summary>
    /// Example cities shown in feature UIs. Any geocodable city works at runtime.
    /// </summary>
    public static IReadOnlyCollection<string> ExampleCities
    {
        get;
    } =
    [
        "London",
        "Madrid",
        "New York",
        "Paris",
        "Tokyo"
    ];

    public async Task<WeatherReading?> GetCurrentAsync(string city, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        GeoLocation? location = await _geocoder.ResolveAsync(city, cancellationToken).ConfigureAwait(false);
        return location is null
            ? null
            : await _forecast.GetCurrentAsync(location, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<WeatherReading>> GetForecastAsync(
        string city,
        int days,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        GeoLocation? location = await _geocoder.ResolveAsync(city, cancellationToken).ConfigureAwait(false);
        if (location is null)
        {
            return [];
        }

        days = Math.Clamp(days, 1, 7);
        return await _forecast.GetDailyAsync(location, days, cancellationToken).ConfigureAwait(false);
    }
}
