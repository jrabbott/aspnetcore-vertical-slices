using WeatherApp.Features.Weather.AddFavorite;
using WeatherApp.Infrastructure.Favorites;
using WeatherApp.Unit.Tests.Fakes;

namespace WeatherApp.Unit.Tests;

public sealed class AddFavoriteHandlerTests
{
    private static AddFavoriteHandler CreateHandler(FavoritesStore store, FakeWeatherClient client)
    {
        return new(store, client, new AddFavoriteRequestValidator());
    }

    [Fact]
    public async Task HandleAsync_WhenCitySupported_AddsFavorite()
    {
        var store = new FavoritesStore([]);
        AddFavoriteHandler handler = CreateHandler(
            store,
            new FakeWeatherClient(FakeWeatherClient.Reading("Madrid", "Spain")));

        AddFavoriteResponse result = await handler.HandleAsync(new AddFavoriteRequest { City = "Madrid" });

        Assert.True(result.Succeeded);
        Assert.Contains("Madrid", result.Message);
        Assert.Equal(["Madrid"], store.GetAll());
    }

    [Fact]
    public async Task HandleAsync_WhenCityUnsupported_Fails()
    {
        var store = new FavoritesStore([]);
        AddFavoriteHandler handler = CreateHandler(store, new FakeWeatherClient());

        AddFavoriteResponse result = await handler.HandleAsync(new AddFavoriteRequest { City = "Atlantis" });

        Assert.False(result.Succeeded);
        Assert.Empty(store.GetAll());
    }

    [Fact]
    public async Task HandleAsync_WhenAlreadyFavorite_Fails()
    {
        var store = new FavoritesStore(["Paris"]);
        AddFavoriteHandler handler = CreateHandler(
            store,
            new FakeWeatherClient(FakeWeatherClient.Reading("Paris", "France")));

        AddFavoriteResponse result = await handler.HandleAsync(new AddFavoriteRequest { City = "Paris" });

        Assert.False(result.Succeeded);
        Assert.Contains("already", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task HandleAsync_WhenCityMissing_FailsValidation()
    {
        var store = new FavoritesStore([]);
        AddFavoriteHandler handler = CreateHandler(store, new FakeWeatherClient());

        AddFavoriteResponse result = await handler.HandleAsync(new AddFavoriteRequest { City = " " });

        Assert.False(result.Succeeded);
        Assert.Equal("Please enter a city name.", result.Message);
        Assert.Empty(store.GetAll());
    }
}
