using WeatherApp.Features.Weather.Search;
using WeatherApp.Unit.Tests.Fakes;

namespace WeatherApp.Unit.Tests;

public sealed class SearchHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenNotSearched_ReturnsEmptyFormState()
    {
        var handler = new SearchHandler(new FakeWeatherClient());

        var response = await handler.HandleAsync(new SearchRequest(), searched: false);

        Assert.False(response.Searched);
        Assert.False(response.Found);
        Assert.Null(response.ErrorMessage);
        Assert.NotEmpty(response.ExampleCities);
    }

    [Fact]
    public async Task HandleAsync_WhenCityMissing_ReturnsValidationError()
    {
        var handler = new SearchHandler(new FakeWeatherClient());

        var response = await handler.HandleAsync(new SearchRequest { City = "  " }, searched: true);

        Assert.True(response.Searched);
        Assert.False(response.Found);
        Assert.Equal("Please enter a city name.", response.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WhenCityUnknown_ReturnsNotFoundMessage()
    {
        var handler = new SearchHandler(new FakeWeatherClient(FakeWeatherClient.Reading("London")));

        var response = await handler.HandleAsync(new SearchRequest { City = "Atlantis" }, searched: true);

        Assert.True(response.Searched);
        Assert.False(response.Found);
        Assert.Contains("Atlantis", response.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WhenCityKnown_ReturnsCurrentWeather()
    {
        var handler = new SearchHandler(
            new FakeWeatherClient(FakeWeatherClient.Reading("Paris", "France", 16, "Partly cloudy")));

        var response = await handler.HandleAsync(new SearchRequest { City = "paris" }, searched: true);

        Assert.True(response.Found);
        Assert.Equal("Paris", response.City);
        Assert.Equal("France", response.Country);
        Assert.Equal(16, response.TemperatureC);
        Assert.Equal("Partly cloudy", response.Summary);
        Assert.Null(response.ErrorMessage);
    }
}
