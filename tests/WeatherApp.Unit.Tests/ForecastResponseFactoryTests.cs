using WeatherApp.Domain.Weather;
using WeatherApp.Features.Weather.Forecast;

namespace WeatherApp.Unit.Tests;

public sealed class ForecastResponseFactoryTests
{
    private static readonly ForecastRequest Request = new() { City = "Paris", Days = 3 };
    private static readonly string[] ExampleCities = ["Paris", "Tokyo"];

    [Fact]
    public void Empty_MarksAsNotSearched()
    {
        var response = ForecastResponse.Empty(Request, ExampleCities);

        Assert.False(response.Searched);
        Assert.Empty(response.Days);
    }

    [Fact]
    public void FromReadings_MapsDaysFromDomainReadings()
    {
        var readings = new[]
        {
            new WeatherReading
            {
                Location = new Location { City = "Paris", Country = "France" },
                Date = new DateOnly(2026, 9, 22),
                TemperatureC = 16,
                Summary = "Sunny",
                HumidityPercent = 40,
                WindSpeedKph = 12
            },
            new WeatherReading
            {
                Location = new Location { City = "Paris", Country = "France" },
                Date = new DateOnly(2026, 9, 23),
                TemperatureC = 17,
                Summary = "Cloudy",
                HumidityPercent = 45,
                WindSpeedKph = 10
            }
        };

        var response = ForecastResponse.FromReadings(Request, readings, ExampleCities);

        Assert.True(response.Found);
        Assert.Equal("Paris", response.City);
        Assert.Equal(2, response.Days.Count);
        Assert.Equal(new DateOnly(2026, 9, 23), response.Days[1].Date);
        Assert.Equal("Cloudy", response.Days[1].Summary);
    }
}
