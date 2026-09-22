using WeatherApp.Features.Weather.Favorites;
using WeatherApp.Infrastructure.Favorites;
using WeatherApp.Unit.Tests.Fakes;

namespace WeatherApp.Unit.Tests;

public sealed class FavoritesHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReturnsFavoritesWithWeatherWhenAvailable()
    {
        var store = new FavoritesStore(["London", "Atlantis"]);
        var weather = new FakeWeatherClient(FakeWeatherClient.Reading("London", "United Kingdom", 12, "Cloudy"));
        var handler = new FavoritesHandler(store, weather);

        FavoritesResponse response = await handler.HandleAsync(new FavoritesRequest());

        Assert.Equal(2, response.Cities.Count);
        Assert.Contains(response.Cities, c => c.City == "London" && c.HasWeather && c.TemperatureC == 12);
        Assert.Contains(response.Cities, c => c.City == "Atlantis" && !c.HasWeather);
        Assert.DoesNotContain(response.SuggestedCities, c => c == "London");
    }
}
