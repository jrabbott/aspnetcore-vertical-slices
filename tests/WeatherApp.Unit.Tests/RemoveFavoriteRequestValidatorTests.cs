using WeatherApp.Features.Weather.RemoveFavorite;

namespace WeatherApp.Unit.Tests;

public sealed class RemoveFavoriteRequestValidatorTests
{
    private readonly RemoveFavoriteRequestValidator _validator = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_WhenCityMissing_Fails(string? city)
    {
        var result = await _validator.ValidateAsync(new RemoveFavoriteRequest { City = city });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "A city is required to remove a favorite.");
    }

    [Fact]
    public async Task Validate_WhenCityValid_Succeeds()
    {
        var result = await _validator.ValidateAsync(new RemoveFavoriteRequest { City = "Tokyo" });

        Assert.True(result.IsValid);
    }
}
