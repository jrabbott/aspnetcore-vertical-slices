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
    public async Task GetForecastAsync_TruncatedDailySeries_SkipsIncompleteDays()
    {
        WeatherClient client = CreateClient(StubHttpMessageHandler.WithTruncatedDaily());

        IReadOnlyList<WeatherReading> forecast = await client.GetForecastAsync("London", days: 3);

        Assert.Single(forecast);
        Assert.Equal(new DateOnly(2026, 9, 22), forecast[0].Date);
        Assert.Equal(16, forecast[0].TemperatureC);
    }

    [Fact]
    public async Task GetCurrentAsync_HttpFailure_ReturnsNull()
    {
        WeatherClient client = CreateClient(StubHttpMessageHandler.Failing());

        WeatherReading? reading = await client.GetCurrentAsync("London");

        Assert.Null(reading);
    }

    [Fact]
    public async Task GetForecastAsync_HttpFailure_ReturnsEmpty()
    {
        WeatherClient client = CreateClient(StubHttpMessageHandler.Failing());

        IReadOnlyList<WeatherReading> forecast = await client.GetForecastAsync("London", days: 3);

        Assert.Empty(forecast);
    }

    [Fact]
    public async Task GetCurrentAsync_SharedGeocoder_CachesAcrossClients()
    {
        var handler = new StubHttpMessageHandler();
        var httpClient = new HttpClient(handler);
        var options = new OpenMeteoOptions();
        var geocoder = new OpenMeteoGeocoder(httpClient, options);
        var first = new WeatherClient(httpClient, geocoder, options);
        var second = new WeatherClient(httpClient, geocoder, options);

        Assert.NotNull(await first.GetCurrentAsync("London"));
        Assert.NotNull(await second.GetCurrentAsync("London"));
        Assert.Equal(1, handler.GeocodeRequestCount);
        Assert.Equal(2, handler.ForecastRequestCount);
    }

    [Fact]
    public async Task GetCurrentAsync_CustomBaseUrls_AreRequested()
    {
        var handler = new StubHttpMessageHandler();
        var options = new OpenMeteoOptions
        {
            GeocodingBaseUrl = "http://wiremock.test/v1/search",
            ForecastBaseUrl = "http://wiremock.test/v1/forecast"
        };
        WeatherClient client = new(new HttpClient(handler), options);

        WeatherReading? reading = await client.GetCurrentAsync("London");

        Assert.NotNull(reading);
        Assert.Equal(1, handler.GeocodeRequestCount);
        Assert.Equal(1, handler.ForecastRequestCount);
        Assert.Contains("wiremock.test/v1/search", handler.LastGeocodeUrl, StringComparison.Ordinal);
        Assert.Contains("wiremock.test/v1/forecast", handler.LastForecastUrl, StringComparison.Ordinal);
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
}
