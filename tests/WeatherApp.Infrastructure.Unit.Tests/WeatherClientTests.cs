using System.Net;
using System.Text;
using WeatherApp.Domain.Weather;
using WeatherApp.Infrastructure.Weather;

namespace WeatherApp.Infrastructure.Unit.Tests;

public sealed class WeatherClientTests
{
    [Theory]
    [InlineData("London")]
    [InlineData("paris")]
    [InlineData("New York")]
    public async Task GetCurrentAsync_KnownCity_ReturnsReading(string city)
    {
        WeatherClient client = CreateClient(new StubHttpMessageHandler());

        WeatherReading? reading = await client.GetCurrentAsync(city);

        Assert.NotNull(reading);
        Assert.False(string.IsNullOrWhiteSpace(reading.Location.City));
        Assert.False(string.IsNullOrWhiteSpace(reading.Summary));
        Assert.False(string.IsNullOrWhiteSpace(reading.Location.Country));
    }

    [Fact]
    public async Task GetCurrentAsync_UnknownCity_ReturnsNull()
    {
        WeatherClient client = CreateClient(new StubHttpMessageHandler());

        WeatherReading? reading = await client.GetCurrentAsync("Atlantis");

        Assert.Null(reading);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetCurrentAsync_BlankCity_ReturnsNull(string? city)
    {
        WeatherClient client = CreateClient(new StubHttpMessageHandler());

        WeatherReading? reading = await client.GetCurrentAsync(city!);

        Assert.Null(reading);
    }

    [Fact]
    public async Task GetForecastAsync_KnownCity_ReturnsDays()
    {
        WeatherClient client = CreateClient(new StubHttpMessageHandler());

        IReadOnlyList<WeatherReading> forecast = await client.GetForecastAsync("Tokyo", days: 4);

        Assert.Equal(4, forecast.Count);
        Assert.All(forecast, day => Assert.Equal("Tokyo", day.Location.City));
    }

    [Fact]
    public async Task GetForecastAsync_UnknownCity_ReturnsEmpty()
    {
        WeatherClient client = CreateClient(new StubHttpMessageHandler());

        IReadOnlyList<WeatherReading> forecast = await client.GetForecastAsync("Nowhere", days: 5);

        Assert.Empty(forecast);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetForecastAsync_BlankCity_ReturnsEmpty(string? city)
    {
        WeatherClient client = CreateClient(new StubHttpMessageHandler());

        IReadOnlyList<WeatherReading> forecast = await client.GetForecastAsync(city!, days: 3);

        Assert.Empty(forecast);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(99, 7)]
    public async Task GetForecastAsync_ClampsDays(int requestedDays, int expectedDays)
    {
        WeatherClient client = CreateClient(new StubHttpMessageHandler());

        IReadOnlyList<WeatherReading> forecast = await client.GetForecastAsync("London", requestedDays);

        Assert.Equal(expectedDays, forecast.Count);
    }

    [Fact]
    public async Task GetCurrentAsync_Canceled_Throws()
    {
        WeatherClient client = CreateClient(new StubHttpMessageHandler());
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => client.GetCurrentAsync("London", cts.Token));
    }

    [Fact]
    public void ExampleCities_ContainsSuggestedCities()
    {
        Assert.Contains("London", WeatherClient.ExampleCities);
        Assert.Contains("Tokyo", WeatherClient.ExampleCities);
        Assert.Equal(5, WeatherClient.ExampleCities.Count);
    }

    private static WeatherClient CreateClient(HttpMessageHandler handler)
    {
        return new(new HttpClient(handler));
    }

    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string url = request.RequestUri?.ToString() ?? string.Empty;

            return url.Contains("geocoding-api.open-meteo.com", StringComparison.Ordinal)
                ? Task.FromResult(JsonResponse(GeocodeJson(url)))
                : url.Contains("api.open-meteo.com", StringComparison.Ordinal)
                ? Task.FromResult(JsonResponse(url.Contains("daily=", StringComparison.Ordinal)
                    ? ForecastDailyJson(url)
                    : CurrentJson()))
                : Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }

        private static string GeocodeJson(string url)
        {
            if (url.Contains("Atlantis", StringComparison.OrdinalIgnoreCase)
                || url.Contains("Nowhere", StringComparison.OrdinalIgnoreCase))
            {
                return """{"results":[]}""";
            }

            string city = "London";
            string country = "United Kingdom";

            if (url.Contains("Paris", StringComparison.OrdinalIgnoreCase))
            {
                city = "Paris";
                country = "France";
            }
            else if (url.Contains("Tokyo", StringComparison.OrdinalIgnoreCase))
            {
                city = "Tokyo";
                country = "Japan";
            }
            else if (url.Contains("New%20York", StringComparison.OrdinalIgnoreCase)
                || url.Contains("New+York", StringComparison.OrdinalIgnoreCase))
            {
                city = "New York";
                country = "United States";
            }
            else if (url.Contains("Madrid", StringComparison.OrdinalIgnoreCase))
            {
                city = "Madrid";
                country = "Spain";
            }

            return $$"""
                {
                  "results": [
                    {
                      "name": "{{city}}",
                      "country": "{{country}}",
                      "latitude": 51.5,
                      "longitude": -0.12
                    }
                  ]
                }
                """;
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

        private static string ForecastDailyJson(string url)
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
}
