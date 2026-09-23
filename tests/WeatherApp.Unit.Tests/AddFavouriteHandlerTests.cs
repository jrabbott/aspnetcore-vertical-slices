using WeatherApp.Features.Weather.AddFavourite;
using WeatherApp.Infrastructure.Favourites;
using WeatherApp.Unit.Tests.Fakes;

namespace WeatherApp.Unit.Tests;

public sealed class AddFavouriteHandlerTests
{
    private static AddFavouriteHandler CreateHandler(FavouritesStore store, FakeWeatherClient client)
    {
        return new(store, client, new AddFavouriteRequestValidator());
    }

    [Fact]
    public async Task HandleAsync_WhenCitySupported_AddsFavourite()
    {
        var store = new FavouritesStore([]);
        AddFavouriteHandler handler = CreateHandler(
            store,
            new FakeWeatherClient(FakeWeatherClient.Reading("Madrid", "Spain")));

        AddFavouriteResponse result = await handler.HandleAsync(new AddFavouriteRequest { City = "Madrid" });

        Assert.True(result.Succeeded);
        Assert.Contains("Madrid", result.Message);
        Assert.Equal(["Madrid"], store.GetAll());
    }

    [Fact]
    public async Task HandleAsync_WhenCityUnsupported_Fails()
    {
        var store = new FavouritesStore([]);
        AddFavouriteHandler handler = CreateHandler(store, new FakeWeatherClient());

        AddFavouriteResponse result = await handler.HandleAsync(new AddFavouriteRequest { City = "Atlantis" });

        Assert.False(result.Succeeded);
        Assert.Empty(store.GetAll());
    }

    [Fact]
    public async Task HandleAsync_WhenAlreadyFavourite_Fails()
    {
        var store = new FavouritesStore(["Paris"]);
        AddFavouriteHandler handler = CreateHandler(
            store,
            new FakeWeatherClient(FakeWeatherClient.Reading("Paris", "France")));

        AddFavouriteResponse result = await handler.HandleAsync(new AddFavouriteRequest { City = "Paris" });

        Assert.False(result.Succeeded);
        Assert.Contains("already", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task HandleAsync_WhenCityMissing_FailsValidation()
    {
        var store = new FavouritesStore([]);
        AddFavouriteHandler handler = CreateHandler(store, new FakeWeatherClient());

        AddFavouriteResponse result = await handler.HandleAsync(new AddFavouriteRequest { City = " " });

        Assert.False(result.Succeeded);
        Assert.Equal("Please enter a city name.", result.Message);
        Assert.Empty(store.GetAll());
    }
}
