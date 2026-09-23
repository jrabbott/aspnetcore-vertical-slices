using WeatherApp.Domain.Weather;

namespace WeatherApp.Domain.Unit.Tests;

public sealed class WeatherReadingTests
{
    [Theory]
    [InlineData(0, 32)]
    [InlineData(100, 212)]
    [InlineData(12, 54)]
    public void TemperatureF_ConvertsFromCelsius(int celsius, int expectedFahrenheit)
    {
        var reading = new WeatherReading
        {
            Location = new Location { City = "Test", Country = "Test" },
            Date = new DateOnly(2026, 9, 22),
            TemperatureC = celsius,
            Summary = "Clear",
            HumidityPercent = 50,
            WindSpeedKph = 10
        };

        Assert.Equal(expectedFahrenheit, reading.TemperatureF);
    }
}

public sealed class LocationTests
{
    [Fact]
    public void Location_RetainsCityAndCountry()
    {
        var location = new Location { City = "Paris", Country = "France" };

        Assert.Equal("Paris", location.City);
        Assert.Equal("France", location.Country);
    }
}
