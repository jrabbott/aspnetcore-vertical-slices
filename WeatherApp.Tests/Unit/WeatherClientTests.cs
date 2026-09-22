using WeatherApp.Infrastructure.Weather;

namespace WeatherApp.Tests.Unit;

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
}
