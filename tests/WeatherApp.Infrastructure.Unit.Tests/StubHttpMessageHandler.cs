using System.Net;
using System.Text;

namespace WeatherApp.Infrastructure.Unit.Tests;

internal sealed class StubHttpMessageHandler : HttpMessageHandler
{
    private static readonly IReadOnlyDictionary<string, (string City, string Country)> _knownCities =
        new Dictionary<string, (string, string)>(StringComparer.OrdinalIgnoreCase)
        {
            ["Paris"] = ("Paris", "France"),
            ["Tokyo"] = ("Tokyo", "Japan"),
            ["New%20York"] = ("New York", "United States"),
            ["New+York"] = ("New York", "United States"),
            ["Madrid"] = ("Madrid", "Spain")
        };

    private static readonly string[] _unknownCities = ["Atlantis", "Nowhere"];

    private readonly bool _truncateDaily;
    private readonly bool _failHttp;

    public StubHttpMessageHandler()
        : this(truncateDaily: false, failHttp: false)
    {
    }

    private StubHttpMessageHandler(bool truncateDaily, bool failHttp)
    {
        _truncateDaily = truncateDaily;
        _failHttp = failHttp;
    }

    public static StubHttpMessageHandler WithTruncatedDaily()
    {
        return new(truncateDaily: true, failHttp: false);
    }

    public static StubHttpMessageHandler Failing()
    {
        return new(truncateDaily: false, failHttp: true);
    }

    public int GeocodeRequestCount
    {
        get; private set;
    }

    public int ForecastRequestCount
    {
        get; private set;
    }

    public string LastGeocodeUrl
    {
        get; private set;
    } = string.Empty;

    public string LastForecastUrl
    {
        get; private set;
    } = string.Empty;

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_failHttp)
        {
            throw new HttpRequestException("Simulated upstream failure.");
        }

        string url = request.RequestUri?.ToString() ?? string.Empty;
        return Task.FromResult(CreateResponse(url));
    }

    private HttpResponseMessage CreateResponse(string url)
    {
        if (IsGeocodeUrl(url))
        {
            GeocodeRequestCount++;
            LastGeocodeUrl = url;
            return JsonResponse(GeocodeJson(url));
        }

        if (IsForecastUrl(url))
        {
            ForecastRequestCount++;
            LastForecastUrl = url;
            string body = url.Contains("daily=", StringComparison.Ordinal)
                ? ForecastDailyJson(url)
                : CurrentJson();
            return JsonResponse(body);
        }

        return new HttpResponseMessage(HttpStatusCode.NotFound);
    }

    private static bool IsGeocodeUrl(string url)
    {
        return url.Contains("geocoding-api.open-meteo.com", StringComparison.Ordinal)
            || url.Contains("/v1/search", StringComparison.Ordinal);
    }

    private static bool IsForecastUrl(string url)
    {
        return url.Contains("api.open-meteo.com", StringComparison.Ordinal)
            || url.Contains("/v1/forecast", StringComparison.Ordinal);
    }

    private static string GeocodeJson(string url)
    {
        if (_unknownCities.Any(city => url.Contains(city, StringComparison.OrdinalIgnoreCase)))
        {
            return """{"results":[]}""";
        }

        (string City, string Country) location = ResolveLocation(url);
        return $$"""
            {
              "results": [
                {
                  "name": "{{location.City}}",
                  "country": "{{location.Country}}",
                  "latitude": 51.5,
                  "longitude": -0.12
                }
              ]
            }
            """;
    }

    private static (string City, string Country) ResolveLocation(string url)
    {
        foreach ((string key, (string city, string country)) in _knownCities)
        {
            if (url.Contains(key, StringComparison.OrdinalIgnoreCase))
            {
                return (city, country);
            }
        }

        return ("London", "United Kingdom");
    }

    private static string CurrentJson()
    {
        return """
            {
              "current": {
                "time": "2026-09-22T12:00",
                "temperature_2m": 12.4,
                "relative_humidity_2m": 78,
                "weather_code": 3,
                "wind_speed_10m": 18.2
              }
            }
            """;
    }

    private string ForecastDailyJson(string url)
    {
        int days = ParseForecastDays(url);
        var times = new List<string>(days);
        var codes = new List<string>(days);
        var temps = new List<string>(days);
        var humidity = new List<string>(days);
        var wind = new List<string>(days);

        for (int i = 0; i < days; i++)
        {
            times.Add($"\"2026-09-{22 + i:00}\"");
            codes.Add("2");
            temps.Add($"{16 + i}.0");
            humidity.Add("55");
            wind.Add("12.0");
        }

        if (_truncateDaily && days > 1)
        {
            codes = [codes[0]];
            temps = [temps[0]];
            humidity = [humidity[0]];
            wind = [wind[0]];
        }

        return $$"""
            {
              "daily": {
                "time": [{{string.Join(",", times)}}],
                "weather_code": [{{string.Join(",", codes)}}],
                "temperature_2m_max": [{{string.Join(",", temps)}}],
                "relative_humidity_2m_mean": [{{string.Join(",", humidity)}}],
                "wind_speed_10m_max": [{{string.Join(",", wind)}}]
              }
            }
            """;
    }

    private static int ParseForecastDays(string url)
    {
        const string key = "forecast_days=";
        int start = url.IndexOf(key, StringComparison.Ordinal);
        if (start < 0)
        {
            return 3;
        }

        start += key.Length;
        int end = start;
        while (end < url.Length && char.IsDigit(url[end]))
        {
            end++;
        }

        return int.TryParse(url[start..end], out int days) ? days : 3;
    }

    private static HttpResponseMessage JsonResponse(string json)
    {
        return new(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
    }
}
