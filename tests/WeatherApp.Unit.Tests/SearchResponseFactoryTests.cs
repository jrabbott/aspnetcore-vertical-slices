using WeatherApp.Domain.Weather;
using WeatherApp.Features.Weather.Search;

namespace WeatherApp.Unit.Tests;

public sealed class SearchResponseFactoryTests
{
    private static readonly SearchRequest Request = new() { City = "London" };
    private static readonly string[] ExampleCities = ["London", "Paris"];

    [Fact]
    public void Empty_MarksAsNotSearched()
    {
        var response = SearchResponse.Empty(Request, ExampleCities);

        Assert.False(response.Searched);
        Assert.False(response.Found);
        Assert.Null(response.ErrorMessage);
        Assert.Equal(ExampleCities, response.ExampleCities);
    }

    [Fact]
    public void Invalid_UsesProvidedErrorMessage()
    {
        var response = SearchResponse.Invalid(Request, "Please enter a city name.", ExampleCities);

        Assert.True(response.Searched);
        Assert.False(response.Found);
        Assert.Equal("Please enter a city name.", response.ErrorMessage);
    }

    [Fact]
    public void NotFound_FormatsUnknownCityMessage()
    {
        var response = SearchResponse.NotFound(Request, "Atlantis", ExampleCities);

        Assert.Contains("Atlantis", response.ErrorMessage);
        Assert.False(response.Found);
    }

    [Fact]
    public void FromReading_MapsWeatherFields()
    {
        var reading = new WeatherReading
        {
            Location = new Location { City = "London", Country = "United Kingdom" },
            Date = new DateOnly(2026, 9, 22),
            TemperatureC = 12,
            Summary = "Cloudy",
            HumidityPercent = 78,
            WindSpeedKph = 18
        };

        var response = SearchResponse.FromReading(Request, reading, ExampleCities);

        Assert.True(response.Found);
        Assert.Equal("London", response.City);
        Assert.Equal("United Kingdom", response.Country);
        Assert.Equal(12, response.TemperatureC);
        Assert.Equal("Cloudy", response.Summary);
        Assert.Null(response.ErrorMessage);
    }
}
