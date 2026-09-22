using WeatherApp.Features.Weather.Search;

namespace WeatherApp.Unit.Tests;

public sealed class SearchRequestValidatorTests
{
    private readonly SearchRequestValidator _validator = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_WhenCityMissing_Fails(string? city)
    {
        var result = await _validator.ValidateAsync(new SearchRequest { City = city });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Please enter a city name.");
    }

    [Fact]
    public async Task Validate_WhenCityTooLong_Fails()
    {
        var result = await _validator.ValidateAsync(new SearchRequest { City = new string('a', 101) });

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            e => e.ErrorMessage == "City name must be between 1 and 100 characters.");
    }

    [Fact]
    public async Task Validate_WhenCityValid_Succeeds()
    {
        var result = await _validator.ValidateAsync(new SearchRequest { City = "London" });

        Assert.True(result.IsValid);
    }
}
