using FluentValidation.Results;
using WeatherApp.Features.Weather.RemoveFavourite;

namespace WeatherApp.Unit.Tests;

public sealed class RemoveFavouriteRequestValidatorTests
{
    private readonly RemoveFavouriteRequestValidator _validator = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_WhenCityMissing_Fails(string? city)
    {
        ValidationResult result = await _validator.ValidateAsync(new RemoveFavouriteRequest { City = city });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "A city is required to remove a favourite.");
    }

    [Fact]
    public async Task Validate_WhenCityValid_Succeeds()
    {
        ValidationResult result = await _validator.ValidateAsync(new RemoveFavouriteRequest { City = "Tokyo" });

        Assert.True(result.IsValid);
    }
}
