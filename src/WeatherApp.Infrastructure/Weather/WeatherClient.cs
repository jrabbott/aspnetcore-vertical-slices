using System.Text.Json;
using Microsoft.Extensions.Options;
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
        : this(httpClient, new OpenMeteoOptions())
    {
    }

    public WeatherClient(HttpClient httpClient, OpenMeteoOptions options)
        : this(httpClient, new OpenMeteoGeocoder(httpClient, options), options)
    {
    }

    public WeatherClient(HttpClient httpClient, IOptions<OpenMeteoOptions> options)
        : this(httpClient, options?.Value ?? throw new ArgumentNullException(nameof(options)))
    {
    }

    internal WeatherClient(HttpClient httpClient, OpenMeteoGeocoder geocoder, OpenMeteoOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(geocoder);
        OpenMeteoOptions resolved = options ?? new OpenMeteoOptions();
        _geocoder = geocoder;
        _forecast = new OpenMeteoForecastClient(httpClient, resolved);
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

        try
        {
            GeoLocation? location = await _geocoder.ResolveAsync(city, cancellationToken).ConfigureAwait(false);
            return location is null
                ? null
                : await _forecast.GetCurrentAsync(location, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex) when (IsTransientWeatherFailure(ex, cancellationToken))
        {
            return null;
        }
    }

    public async Task<IReadOnlyList<WeatherReading>> GetForecastAsync(
        string city,
        int days,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            GeoLocation? location = await _geocoder.ResolveAsync(city, cancellationToken).ConfigureAwait(false);
            if (location is null)
            {
                return [];
            }

            days = Math.Clamp(days, 1, 7);
            return await _forecast.GetDailyAsync(location, days, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex) when (IsTransientWeatherFailure(ex, cancellationToken))
        {
            return [];
        }
    }

    private static bool IsTransientWeatherFailure(Exception exception, CancellationToken cancellationToken)
    {
        return exception is HttpRequestException or JsonException
            || (exception is OperationCanceledException && !cancellationToken.IsCancellationRequested);
    }
}
