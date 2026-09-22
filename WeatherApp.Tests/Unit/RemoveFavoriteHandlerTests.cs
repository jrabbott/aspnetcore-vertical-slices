using WeatherApp.Features.Weather.RemoveFavorite;
using WeatherApp.Infrastructure.Favorites;

namespace WeatherApp.Tests.Unit;

public sealed class RemoveFavoriteHandlerTests
{
    [Fact]
    public void Handle_WhenCityPresent_RemovesFavorite()
    {
        var store = new FavoritesStore(["London", "Tokyo"]);
        var handler = new RemoveFavoriteHandler(store);

        var result = handler.Handle(new RemoveFavoriteRequest { City = "London" });

        Assert.True(result.Succeeded);
        Assert.Equal(["Tokyo"], store.GetAll());
    }

    [Fact]
    public void Handle_WhenCityMissing_Fails()
    {
        var store = new FavoritesStore(["Tokyo"]);
        var handler = new RemoveFavoriteHandler(store);

        var result = handler.Handle(new RemoveFavoriteRequest { City = "Paris" });

        Assert.False(result.Succeeded);
        Assert.Equal(["Tokyo"], store.GetAll());
    }
}
