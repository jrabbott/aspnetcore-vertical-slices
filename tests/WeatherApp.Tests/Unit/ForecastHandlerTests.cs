using WeatherApp.Features.Weather.Forecast;
using WeatherApp.Tests.Fakes;

namespace WeatherApp.Tests.Unit;

public sealed class ForecastHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenCityKnown_ReturnsRequestedNumberOfDays()
    {
        var handler = new ForecastHandler(
            new FakeWeatherClient(FakeWeatherClient.Reading("Tokyo", "Japan", 22, "Humid")));

        var response = await handler.HandleAsync(
            new ForecastRequest { City = "Tokyo", Days = 3 },
            searched: true);

        Assert.True(response.Found);
        Assert.Equal("Tokyo", response.City);
        Assert.Equal(3, response.Days.Count);
        Assert.Equal(new DateOnly(2026, 9, 22), response.Days[0].Date);
        Assert.Equal(new DateOnly(2026, 9, 24), response.Days[2].Date);
    }

    [Fact]
    public async Task HandleAsync_WhenCityUnknown_ReturnsError()
    {
        var handler = new ForecastHandler(new FakeWeatherClient());

        var response = await handler.HandleAsync(
            new ForecastRequest { City = "Nowhere" },
            searched: true);

        Assert.False(response.Found);
        Assert.Contains("Nowhere", response.ErrorMessage);
        Assert.Empty(response.Days);
    }
}
