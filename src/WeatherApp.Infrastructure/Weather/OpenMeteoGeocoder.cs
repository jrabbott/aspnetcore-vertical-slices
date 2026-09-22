using System.Collections.Concurrent;
using System.Net.Http.Json;

namespace WeatherApp.Infrastructure.Weather;

internal sealed class OpenMeteoGeocoder(HttpClient httpClient)
{
    private const string GeocodingBase = "https://geocoding-api.open-meteo.com/v1/search";

    private readonly HttpClient _httpClient = httpClient;
    private readonly ConcurrentDictionary<string, GeoLocation?> _cache = new(StringComparer.OrdinalIgnoreCase);

    public async Task<GeoLocation?> ResolveAsync(string city, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            return null;
        }

        string key = city.Trim();

        if (_cache.TryGetValue(key, out GeoLocation? cached))
        {
            return cached;
        }

        string url =
            $"{GeocodingBase}?name={Uri.EscapeDataString(key)}&count=1&language=en&format=json";

        GeocodingResponse? response = await _httpClient
            .GetFromJsonAsync<GeocodingResponse>(url, cancellationToken)
            .ConfigureAwait(false);

        GeocodingResult? match = response?.Results?.FirstOrDefault();
        GeoLocation? location = match is null
            ? null
            : new GeoLocation(
                match.Name,
                string.IsNullOrWhiteSpace(match.Country) ? "Unknown" : match.Country,
                match.Latitude,
                match.Longitude);

        _cache[key] = location;
        return location;
    }
}

internal sealed record GeoLocation(string City, string Country, double Latitude, double Longitude);
