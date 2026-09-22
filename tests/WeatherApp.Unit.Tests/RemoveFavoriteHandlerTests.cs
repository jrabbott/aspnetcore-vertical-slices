using WeatherApp.Features.Weather.RemoveFavorite;
using WeatherApp.Infrastructure.Favorites;

namespace WeatherApp.Unit.Tests;

public sealed class RemoveFavoriteHandlerTests
{
    private static RemoveFavoriteHandler CreateHandler(FavoritesStore store) =>
        new(store, new RemoveFavoriteRequestValidator());

    [Fact]
    public async Task HandleAsync_WhenCityPresent_RemovesFavorite()
    {
        var store = new FavoritesStore(["London", "Tokyo"]);
        var handler = CreateHandler(store);

        var result = await handler.HandleAsync(new RemoveFavoriteRequest { City = "London" });

        Assert.True(result.Succeeded);
        Assert.Equal(["Tokyo"], store.GetAll());
    }

    [Fact]
    public async Task HandleAsync_WhenCityMissingFromStore_Fails()
    {
        var store = new FavoritesStore(["Tokyo"]);
        var handler = CreateHandler(store);

        var result = await handler.HandleAsync(new RemoveFavoriteRequest { City = "Paris" });

        Assert.False(result.Succeeded);
        Assert.Equal(["Tokyo"], store.GetAll());
    }

    [Fact]
    public async Task HandleAsync_WhenCityBlank_FailsValidation()
    {
        var store = new FavoritesStore(["Tokyo"]);
        var handler = CreateHandler(store);

        var result = await handler.HandleAsync(new RemoveFavoriteRequest { City = " " });

        Assert.False(result.Succeeded);
        Assert.Equal("A city is required to remove a favorite.", result.Message);
        Assert.Equal(["Tokyo"], store.GetAll());
    }
}
