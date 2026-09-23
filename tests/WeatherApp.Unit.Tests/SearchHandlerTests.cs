using WeatherApp.Features.Weather.Search;
using WeatherApp.TestSupport;

namespace WeatherApp.Unit.Tests;

public sealed class SearchHandlerTests
{
    private static SearchHandler CreateHandler(FakeWeatherClient client)
    {
        return new(client, new SearchRequestValidator());
    }

    [Fact]
    public async Task HandleAsync_WhenNotSearched_ReturnsEmptyFormState()
    {
        SearchHandler handler = CreateHandler(new FakeWeatherClient());

        SearchResponse response = await handler.HandleAsync(new SearchRequest(), searched: false);

        Assert.False(response.Searched);
        Assert.False(response.Found);
        Assert.Null(response.ErrorMessage);
        Assert.NotEmpty(response.ExampleCities);
    }

    [Fact]
    public async Task HandleAsync_WhenCityMissing_ReturnsValidationError()
    {
        SearchHandler handler = CreateHandler(new FakeWeatherClient());

        SearchResponse response = await handler.HandleAsync(new SearchRequest { City = "  " }, searched: true);

        Assert.True(response.Searched);
        Assert.False(response.Found);
        Assert.Equal("Please enter a city name.", response.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WhenCityUnknown_ReturnsNotFoundMessage()
    {
        SearchHandler handler = CreateHandler(new FakeWeatherClient(FakeWeatherClient.Reading("London")));

        SearchResponse response = await handler.HandleAsync(new SearchRequest { City = "Atlantis" }, searched: true);

        Assert.True(response.Searched);
        Assert.False(response.Found);
        Assert.Contains("Atlantis", response.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WhenCityKnown_ReturnsCurrentWeather()
    {
        SearchHandler handler = CreateHandler(
            new FakeWeatherClient(FakeWeatherClient.Reading("Paris", "France", 16, "Partly cloudy")));

        SearchResponse response = await handler.HandleAsync(new SearchRequest { City = "paris" }, searched: true);

        Assert.True(response.Found);
        Assert.Equal("Paris", response.City);
        Assert.Equal("France", response.Country);
        Assert.Equal(16, response.TemperatureC);
        Assert.Equal("Partly cloudy", response.Summary);
        Assert.Null(response.ErrorMessage);
    }
}
