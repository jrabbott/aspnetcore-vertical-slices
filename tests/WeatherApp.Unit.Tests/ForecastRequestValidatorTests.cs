using FluentValidation.Results;
using WeatherApp.Features.Weather.Forecast;

namespace WeatherApp.Unit.Tests;

public sealed class ForecastRequestValidatorTests
{
    private readonly ForecastRequestValidator _validator = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_WhenCityMissing_Fails(string? city)
    {
        ValidationResult result = await _validator.ValidateAsync(new ForecastRequest { City = city, Days = 5 });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Please enter a city name.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(8)]
    [InlineData(-1)]
    public async Task Validate_WhenDaysOutOfRange_Fails(int days)
    {
        ValidationResult result = await _validator.ValidateAsync(new ForecastRequest { City = "Paris", Days = days });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Days must be between 1 and 7.");
    }

    [Fact]
    public async Task Validate_WhenCityTooLong_Fails()
    {
        ValidationResult result = await _validator.ValidateAsync(
            new ForecastRequest { City = new string('a', 101), Days = 3 });

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            e => e.ErrorMessage == "City name must be between 1 and 100 characters.");
    }

    [Fact]
    public async Task Validate_WhenRequestValid_Succeeds()
    {
        ValidationResult result = await _validator.ValidateAsync(new ForecastRequest { City = "Paris", Days = 3 });

        Assert.True(result.IsValid);
    }
}
