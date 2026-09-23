using WeatherApp.Features.Weather.Forecast;
using WeatherApp.TestSupport;

namespace WeatherApp.Unit.Tests;

public sealed class ForecastHandlerTests
{
    private static ForecastHandler CreateHandler(FakeWeatherClient client)
    {
        return new(client, new ForecastRequestValidator());
    }

    [Fact]
    public async Task HandleAsync_WhenCityKnown_ReturnsRequestedNumberOfDays()
    {
        ForecastHandler handler = CreateHandler(
            new FakeWeatherClient(FakeWeatherClient.Reading("Tokyo", "Japan", 22, "Humid")));

        ForecastResponse response = await handler.HandleAsync(
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
        ForecastHandler handler = CreateHandler(new FakeWeatherClient());

        ForecastResponse response = await handler.HandleAsync(
            new ForecastRequest { City = "Nowhere" },
            searched: true);

        Assert.False(response.Found);
        Assert.Contains("Nowhere", response.ErrorMessage);
        Assert.Empty(response.Days);
    }

    [Fact]
    public async Task HandleAsync_WhenDaysOutOfRange_ReturnsValidationError()
    {
        ForecastHandler handler = CreateHandler(new FakeWeatherClient(FakeWeatherClient.Reading("Paris")));

        ForecastResponse response = await handler.HandleAsync(
            new ForecastRequest { City = "Paris", Days = 30 },
            searched: true);

        Assert.False(response.Found);
        Assert.Equal("Days must be between 1 and 7.", response.ErrorMessage);
    }
}
