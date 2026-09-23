using WeatherApp.Features.Weather.Favourites;
using WeatherApp.TestSupport;

namespace WeatherApp.Unit.Tests;

public sealed class FavouritesHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReturnsFavouritesWithWeatherWhenAvailable()
    {
        var store = new FakeFavouritesStore(["London", "Atlantis"]);
        var weather = new FakeWeatherClient(FakeWeatherClient.Reading("London", "United Kingdom", 12, "Cloudy"));
        var handler = new FavouritesHandler(store, weather);

        FavouritesResponse response = await handler.HandleAsync(new FavouritesRequest());

        Assert.Equal(2, response.Cities.Count);
        Assert.Contains(response.Cities, c => c.City == "London" && c.HasWeather && c.TemperatureC == 12);
        Assert.Contains(response.Cities, c => c.City == "Atlantis" && !c.HasWeather);
        Assert.DoesNotContain(response.SuggestedCities, c => c == "London");
    }
}
