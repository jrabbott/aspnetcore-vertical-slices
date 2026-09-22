using System.Net;

namespace WeatherApp.Integration.Tests;

public sealed class FavoritesEndpointTests : IClassFixture<WeatherAppFactory>
{
    private readonly WeatherAppFactory _factory;

    public FavoritesEndpointTests(WeatherAppFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Favorites_EmptyStore_ShowsEmptyState()
    {
        var client = _factory.CreateClientWithFavorites([]);
        var response = await client.GetAsync("/weather/favorites");
        var document = await HtmlDocument.ParseAsync(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("You have no favorite cities yet", document.QuerySelector("p.empty")?.TextContent);
        Assert.Equal("Add a favorite", document.QuerySelector("section.add-favorite h2")?.TextContent.Trim());
        Assert.Empty(document.QuerySelectorAll("ul.favorites-list > li"));
    }

    [Fact]
    public async Task Favorites_CityWithoutWeather_ShowsUnavailableMessage()
    {
        var client = _factory.CreateClientWithFavorites(["Atlantis"]);
        var response = await client.GetAsync("/weather/favorites");
        var document = await HtmlDocument.ParseAsync(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(
            document.QuerySelectorAll("ul.favorites-list > li h2"),
            heading => heading.TextContent.Trim() == "Atlantis");
        Assert.Contains(
            "Weather unavailable for this city.",
            document.QuerySelector("p.muted")?.TextContent);
    }

    [Fact]
    public async Task AddAndRemoveFavorite_RoundTrip()
    {
        var client = _factory.CreateClientWithFavorites([]);
        var page = await client.GetAsync("/weather/favorites");
        var document = await HtmlDocument.ParseAsync(page);
        var token = HtmlDocument.AntiForgeryToken(document);

        var addResponse = await client.PostAsync(
            "/weather/favorites/add",
            Form(token, "Madrid"));

        Assert.Equal(HttpStatusCode.Redirect, addResponse.StatusCode);
        Assert.Equal("/weather/favorites", addResponse.Headers.Location?.ToString());

        var afterAdd = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favorites"));
        Assert.Contains("Madrid was added", afterAdd.QuerySelector(".alert.alert-success")?.TextContent);
        Assert.Contains(
            afterAdd.QuerySelectorAll("ul.favorites-list > li h2"),
            heading => heading.TextContent.Trim() == "Madrid");

        var removeToken = HtmlDocument.AntiForgeryToken(afterAdd);
        var removeResponse = await client.PostAsync(
            "/weather/favorites/remove",
            Form(removeToken, "Madrid"));

        Assert.Equal(HttpStatusCode.Redirect, removeResponse.StatusCode);

        var afterRemove = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favorites"));
        Assert.Contains("Madrid was removed", afterRemove.QuerySelector(".alert.alert-success")?.TextContent);
        Assert.DoesNotContain(
            afterRemove.QuerySelectorAll("ul.favorites-list > li h2"),
            heading => heading.TextContent.Trim() == "Madrid");
    }

    [Fact]
    public async Task AddFavorite_DuplicateCity_ShowsError()
    {
        var client = _factory.CreateClientWithFavorites(["London"]);
        var page = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favorites"));
        var token = HtmlDocument.AntiForgeryToken(page);

        var response = await client.PostAsync("/weather/favorites/add", Form(token, "London"));
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        var document = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favorites"));
        Assert.Contains(
            "already in your favorites",
            document.QuerySelector(".alert.alert-error")?.TextContent);
    }

    [Fact]
    public async Task AddFavorite_UnsupportedCity_ShowsError()
    {
        var client = _factory.CreateClientWithFavorites([]);
        var page = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favorites"));
        var token = HtmlDocument.AntiForgeryToken(page);

        var response = await client.PostAsync("/weather/favorites/add", Form(token, "Atlantis"));
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        var document = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favorites"));
        Assert.Contains(
            "not a supported city",
            document.QuerySelector(".alert.alert-error")?.TextContent);
        Assert.Empty(document.QuerySelectorAll("ul.favorites-list > li"));
    }

    [Fact]
    public async Task AddFavorite_BlankCity_ShowsError()
    {
        var client = _factory.CreateClientWithFavorites([]);
        var page = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favorites"));
        var token = HtmlDocument.AntiForgeryToken(page);

        var response = await client.PostAsync("/weather/favorites/add", Form(token, ""));
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        var document = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favorites"));
        Assert.Contains(
            "Please enter a city name.",
            document.QuerySelector(".alert.alert-error")?.TextContent);
    }

    [Fact]
    public async Task RemoveFavorite_MissingCity_ShowsError()
    {
        var client = _factory.CreateClientWithFavorites([]);
        var page = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favorites"));
        var token = HtmlDocument.AntiForgeryToken(page);

        var response = await client.PostAsync("/weather/favorites/remove", Form(token, "Madrid"));
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        var document = await HtmlDocument.ParseAsync(await client.GetAsync("/weather/favorites"));
        Assert.Contains(
            "was not in your favorites",
            document.QuerySelector(".alert.alert-error")?.TextContent);
    }

    private static FormUrlEncodedContent Form(string token, string city) =>
        new(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["City"] = city
        });
}
