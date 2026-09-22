using WeatherApp.Features.Weather.AddFavorite;
using WeatherApp.Infrastructure.Favorites;
using WeatherApp.Tests.Fakes;

namespace WeatherApp.Tests.Unit;

public sealed class AddFavoriteHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenCitySupported_AddsFavorite()
    {
        var store = new FavoritesStore([]);
        var handler = new AddFavoriteHandler(
            store,
            new FakeWeatherClient(FakeWeatherClient.Reading("Madrid", "Spain")));

        var result = await handler.HandleAsync(new AddFavoriteRequest { City = "Madrid" });

        Assert.True(result.Succeeded);
        Assert.Contains("Madrid", result.Message);
        Assert.Equal(["Madrid"], store.GetAll());
    }

    [Fact]
    public async Task HandleAsync_WhenCityUnsupported_Fails()
    {
        var store = new FavoritesStore([]);
        var handler = new AddFavoriteHandler(store, new FakeWeatherClient());

        var result = await handler.HandleAsync(new AddFavoriteRequest { City = "Atlantis" });

        Assert.False(result.Succeeded);
        Assert.Empty(store.GetAll());
    }

    [Fact]
    public async Task HandleAsync_WhenAlreadyFavorite_Fails()
    {
        var store = new FavoritesStore(["Paris"]);
        var handler = new AddFavoriteHandler(
            store,
            new FakeWeatherClient(FakeWeatherClient.Reading("Paris", "France")));

        var result = await handler.HandleAsync(new AddFavoriteRequest { City = "Paris" });

        Assert.False(result.Succeeded);
        Assert.Contains("already", result.Message, StringComparison.OrdinalIgnoreCase);
    }
}
