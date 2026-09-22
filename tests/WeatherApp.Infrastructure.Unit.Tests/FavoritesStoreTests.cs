using WeatherApp.Infrastructure.Favorites;

namespace WeatherApp.Infrastructure.Unit.Tests;

public sealed class FavoritesStoreTests
{
    [Fact]
    public void Add_ThenGetAll_ReturnsSortedCities()
    {
        var store = new FavoritesStore([]);

        Assert.True(store.Add("Tokyo"));
        Assert.True(store.Add("London"));

        Assert.Equal(["London", "Tokyo"], store.GetAll());
    }

    [Fact]
    public void Add_DuplicateCity_ReturnsFalse()
    {
        var store = new FavoritesStore(["Paris"]);

        Assert.False(store.Add("paris"));
        Assert.Equal(["Paris"], store.GetAll());
    }

    [Fact]
    public void Remove_ExistingCity_ReturnsTrue()
    {
        var store = new FavoritesStore(["London", "Tokyo"]);

        Assert.True(store.Remove("London"));
        Assert.Equal(["Tokyo"], store.GetAll());
    }

    [Fact]
    public void Remove_MissingCity_ReturnsFalse()
    {
        var store = new FavoritesStore(["Tokyo"]);

        Assert.False(store.Remove("Paris"));
        Assert.Equal(["Tokyo"], store.GetAll());
    }
}
