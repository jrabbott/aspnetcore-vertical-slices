using WeatherApp.TestSupport;

namespace WeatherApp.Unit.Tests;

public sealed class FakeFavouritesStoreTests
{
    [Fact]
    public void DefaultConstructor_SeedsLondonAndTokyo()
    {
        var store = new FakeFavouritesStore();

        Assert.Equal(["London", "Tokyo"], store.GetAll());
    }

    [Fact]
    public void Constructor_TrimsFiltersBlanksAndDedupes()
    {
        var store = new FakeFavouritesStore(["  Paris ", "", "paris", "  ", "Madrid"]);

        Assert.Equal(["Madrid", "Paris"], store.GetAll());
    }

    [Fact]
    public void Add_ThenGetAll_ReturnsSortedCities()
    {
        var store = new FakeFavouritesStore([]);

        Assert.True(store.Add("Tokyo"));
        Assert.True(store.Add("London"));

        Assert.Equal(["London", "Tokyo"], store.GetAll());
    }

    [Fact]
    public void Add_DuplicateCity_ReturnsFalse()
    {
        var store = new FakeFavouritesStore(["Paris"]);

        Assert.False(store.Add("paris"));
        Assert.Equal(["Paris"], store.GetAll());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Add_BlankCity_ReturnsFalse(string? city)
    {
        var store = new FakeFavouritesStore([]);

        Assert.False(store.Add(city!));
        Assert.Empty(store.GetAll());
    }

    [Fact]
    public void Remove_ExistingCity_ReturnsTrue()
    {
        var store = new FakeFavouritesStore(["London", "Tokyo"]);

        Assert.True(store.Remove("London"));
        Assert.Equal(["Tokyo"], store.GetAll());
    }

    [Fact]
    public void Remove_MissingCity_ReturnsFalse()
    {
        var store = new FakeFavouritesStore(["Tokyo"]);

        Assert.False(store.Remove("Paris"));
        Assert.Equal(["Tokyo"], store.GetAll());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Remove_BlankCity_ReturnsFalse(string? city)
    {
        var store = new FakeFavouritesStore(["Tokyo"]);

        Assert.False(store.Remove(city!));
        Assert.Equal(["Tokyo"], store.GetAll());
    }
}
