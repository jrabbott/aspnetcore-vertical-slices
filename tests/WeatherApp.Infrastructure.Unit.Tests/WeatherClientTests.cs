using WeatherApp.Infrastructure.Weather;

namespace WeatherApp.Infrastructure.Unit.Tests;

public sealed class WeatherClientTests
{
    private readonly WeatherClient _client = new();

    [Theory]
    [InlineData("London")]
    [InlineData("paris")]
    [InlineData("New York")]
    public async Task GetCurrentAsync_KnownCity_ReturnsReading(string city)
    {
        var reading = await _client.GetCurrentAsync(city);

        Assert.NotNull(reading);
        Assert.False(string.IsNullOrWhiteSpace(reading.Location.City));
        Assert.False(string.IsNullOrWhiteSpace(reading.Summary));
    }

    [Fact]
    public async Task GetCurrentAsync_UnknownCity_ReturnsNull()
    {
        var reading = await _client.GetCurrentAsync("Atlantis");

        Assert.Null(reading);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetCurrentAsync_BlankCity_ReturnsNull(string? city)
    {
        var reading = await _client.GetCurrentAsync(city!);

        Assert.Null(reading);
    }

    [Fact]
    public async Task GetForecastAsync_KnownCity_ReturnsDays()
    {
        var forecast = await _client.GetForecastAsync("Tokyo", days: 4);

        Assert.Equal(4, forecast.Count);
        Assert.All(forecast, day => Assert.Equal("Tokyo", day.Location.City));
    }

    [Fact]
    public async Task GetForecastAsync_UnknownCity_ReturnsEmpty()
    {
        var forecast = await _client.GetForecastAsync("Nowhere", days: 5);

        Assert.Empty(forecast);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetForecastAsync_BlankCity_ReturnsEmpty(string? city)
    {
        var forecast = await _client.GetForecastAsync(city!, days: 3);

        Assert.Empty(forecast);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(99, 7)]
    public async Task GetForecastAsync_ClampsDays(int requestedDays, int expectedDays)
    {
        var forecast = await _client.GetForecastAsync("London", requestedDays);

        Assert.Equal(expectedDays, forecast.Count);
    }

    [Fact]
    public async Task GetCurrentAsync_Canceled_Throws()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => _client.GetCurrentAsync("London", cts.Token));
    }

    [Fact]
    public void KnownCities_ContainsSupportedCities()
    {
        Assert.Contains("London", WeatherClient.KnownCities);
        Assert.Contains("Tokyo", WeatherClient.KnownCities);
        Assert.Equal(5, WeatherClient.KnownCities.Count);
    }
}
