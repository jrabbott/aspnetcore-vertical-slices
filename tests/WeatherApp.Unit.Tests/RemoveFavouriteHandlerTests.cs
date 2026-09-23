using WeatherApp.Features.Weather.RemoveFavourite;
using WeatherApp.Infrastructure.Favourites;

namespace WeatherApp.Unit.Tests;

public sealed class RemoveFavouriteHandlerTests
{
    private static RemoveFavouriteHandler CreateHandler(FavouritesStore store)
    {
        return new(store, new RemoveFavouriteRequestValidator());
    }

    [Fact]
    public async Task HandleAsync_WhenCityPresent_RemovesFavourite()
    {
        var store = new FavouritesStore(["London", "Tokyo"]);
        RemoveFavouriteHandler handler = CreateHandler(store);

        RemoveFavouriteResponse result = await handler.HandleAsync(new RemoveFavouriteRequest { City = "London" });

        Assert.True(result.Succeeded);
        Assert.Equal(["Tokyo"], store.GetAll());
    }

    [Fact]
    public async Task HandleAsync_WhenCityMissingFromStore_Fails()
    {
        var store = new FavouritesStore(["Tokyo"]);
        RemoveFavouriteHandler handler = CreateHandler(store);

        RemoveFavouriteResponse result = await handler.HandleAsync(new RemoveFavouriteRequest { City = "Paris" });

        Assert.False(result.Succeeded);
        Assert.Equal(["Tokyo"], store.GetAll());
    }

    [Fact]
    public async Task HandleAsync_WhenCityBlank_FailsValidation()
    {
        var store = new FavouritesStore(["Tokyo"]);
        RemoveFavouriteHandler handler = CreateHandler(store);

        RemoveFavouriteResponse result = await handler.HandleAsync(new RemoveFavouriteRequest { City = " " });

        Assert.False(result.Succeeded);
        Assert.Equal("A city is required to remove a favourite.", result.Message);
        Assert.Equal(["Tokyo"], store.GetAll());
    }
}
